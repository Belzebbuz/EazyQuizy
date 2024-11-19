using Microsoft.Extensions.Configuration;

namespace EazyQuizy.Common.HashiVault;

/// <summary>
/// Поставщик данных из корпоративного хранилища секретов Vault
/// </summary>
public class HashiCorpVaultConfigurationProvider : ConfigurationProvider
{
	/// <summary>
	/// Окончание наименования ключа, которое должно содержать ссылочное свойство в конфигурации
	/// </summary>
	private const string EndCommand = "-secret";

	/// <inheritdoc cref="HashiCorpVaultConfigurationSource"/>
	private readonly HashiCorpVaultConfigurationSource _source;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="source">Источник для поставщика данных из корпоративного хранилища секретов Vault</param>
	public HashiCorpVaultConfigurationProvider(HashiCorpVaultConfigurationSource source)
	{
		_source = source;
	}

	/// <summary>
	/// Загружает секреты из корпоративного хранилища в Vault в конфигурацию
	/// </summary>
	public override void Load()
	{
		if (_source.Secrets.Count == 0)
			return;

		foreach(ReferenceVaultSecretConfig secretConfig in _source.Secrets.Values)
			DownloadSecret(secretConfig);
	}

	/// <summary>
	/// Добавляет значение в конфигурацию. Если ключ содержит "-VaultSecret", парсит значение строку
	/// </summary>
	/// <inheritdoc />
	public override void Set(string key, string? value)
	{
		// Если это не наш секрет, то нефиг провайдеру его обрабатывать
		if (string.IsNullOrEmpty(value) || !key.EndsWith(EndCommand, StringComparison.OrdinalIgnoreCase))
			return;

		_source.Secrets[key] = Parse(key, value); ;
		base.Set(key, value);
	}

	/// <summary>
	/// Загружает секрет в конфигурацию 
	/// </summary>
	/// <param name="secretConfig">Параметры секрета</param>
	private void DownloadSecret(ReferenceVaultSecretConfig secretConfig)
	{
		var secrets = _source
			.Client
			.GetSecretValuesAsync(secretConfig.SecretPath).Result;

		Data[secretConfig.PropertyPath] = secrets[secretConfig.SecretKey].ToString();
	}

	/// <summary>
	/// Парсит референсное свойство секрета Vault
	/// </summary>
	/// <param name="key">Ключ</param>
	/// <param name="value">Значение</param>
	/// <returns>Ссылочное свойство конфигурации ссылающееся на секрет Vault</returns>
	private ReferenceVaultSecretConfig Parse(string key, string value)
	{
		var valueIndex = value.LastIndexOf('/');
		return new ReferenceVaultSecretConfig
		{
			PropertyPath = key[..^EndCommand.Length],
			SecretPath = value[..valueIndex],
			SecretKey = value[++valueIndex..],
		};
	}
}