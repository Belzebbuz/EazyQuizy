namespace EazyQuizy.Api.Configs.HashiVault;

/// <summary>
/// Источник для провайдера данных из корпоративного хранилища секретов Vault
/// </summary>
public class HashiCorpVaultConfigurationSource : IConfigurationSource
{
	/// <inheritdoc cref="IHashiCorpVaultClient"/>
	public required IHashiCorpVaultClient Client { get; set; }

	/// <summary>
	/// Коллекция секретов, которые нужно загрузить в конфиг
	/// </summary>
	public required IDictionary<string, ReferenceVaultSecretConfig> Secrets { get; set; }

	/// <inheritdoc />
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new HashiCorpVaultConfigurationProvider(this);
	}
}