using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Throw;

namespace EazyQuizy.Common.HashiVault;

public static class HostBuilderExtensions
{
	
	/// <summary>
	/// Настраивает приложение на работу с корпоративным хранилищем секретов
	/// </summary>
	/// <param name="hostBuilder">Строитель хоста</param>
	/// <param name="vaultSection">Наименование секции с конфигурацией <see cref="HashiCorpVaultConfig"/></param>
	/// <returns><paramref name="hostBuilder"/></returns>
	public static IHostBuilder UseHashiCorpVault(
		this IHostBuilder hostBuilder,
		string vaultSection = "HashiCorpVaultConfig")
	{
		// Ссылку оставляем здесь так как нам нужно обеспечить единый клиент на разных этапах сборки приложения
		IHashiCorpVaultClient? vaultClient = default;
		var secrets = new Dictionary<string, ReferenceVaultSecretConfig>(StringComparer.OrdinalIgnoreCase);

		return hostBuilder
			.ConfigureHostConfiguration(builder =>
			{
				IConfigurationRoot configuration = builder.Build();

				HashiCorpVaultConfig vaultConfig = configuration
					.GetSection(vaultSection)
					.Get<HashiCorpVaultConfig>().ThrowIfNull();

				var options = Options.Create(vaultConfig);
				vaultClient = new HashiCorpVaultClient(options);

				// builder.Sources.Clear();
				// builder.AddConfiguration(configuration);
				builder.Add(new HashiCorpVaultConfigurationSource
					{
						Client = vaultClient,
						Secrets = secrets,
					});
			})
			.ConfigureAppConfiguration((hostContext, builder) =>
			{
				// Загружаем все секреты запрошенные на этапе создания конфигурации хоста
				if (hostContext.Configuration is IConfigurationRoot configurationRoot)
					configurationRoot.ReloadHashiCorpVaultSecrets();

				vaultClient.ThrowIfNull();
				builder.Add(new HashiCorpVaultConfigurationSource
				{
					Client = vaultClient,
					Secrets = secrets,
				});
			})
			.ConfigureServices((hostContext, services) =>
			{
				// Загружаем все секреты запрошенные на этапе создания конфигурации приложения
				if (hostContext.Configuration is IConfigurationRoot configurationRoot)
					configurationRoot.ReloadHashiCorpVaultSecrets();

				services
					.AddSingleton<IHashiCorpVaultClient, HashiCorpVaultClient>();
			});
	}

	/// <summary>
	/// Перезагружает секреты в конфигурации
	/// </summary>
	/// <param name="configuration">Конфигурация приложения</param>
	private static IConfigurationRoot ReloadHashiCorpVaultSecrets(this IConfigurationRoot configuration)
	{
		foreach (var provider in configuration.Providers)
			if (provider is HashiCorpVaultConfigurationProvider)
			{
				foreach (var (key, value) in configuration.AsEnumerable())
					provider.Set(key, value);

				provider.Load();
			}

		return configuration;
	}
}