namespace EazyQuizy.Common.HashiVault;

/// <summary>
/// Ссылочное свойство конфигурации, ссылающееся на секрет Vault
/// </summary>
public class ReferenceVaultSecretConfig
{
	/// <summary>
	/// Путь к секрету Vault
	/// </summary>
	public required string SecretPath { get; init; }

	/// <summary>
	/// Ключ секрета Vault
	/// </summary>
	public required string SecretKey { get; init; }

	/// <summary>
	/// Свойство, в которое будет записан секрет Vault
	/// </summary>
	public required string PropertyPath { get; init; }
}