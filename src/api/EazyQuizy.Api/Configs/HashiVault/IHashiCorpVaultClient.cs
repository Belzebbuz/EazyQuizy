namespace EazyQuizy.Api.Configs.HashiVault;

/// <summary>
/// Клиент для работы с HashiCorpVault
/// </summary>
public interface IHashiCorpVaultClient
{
	/// <summary>
	/// Возвращает результат выполнения запроса к хранилищу со словарём всех доступных значений для секрета
	/// </summary>
	/// <param name="secretPath">Путь к секрету</param>
	/// <returns>Результат выполнения запроса к хранилищу со словарём всех доступных значений для секрета</returns>
	Task<IDictionary<string, object>> GetSecretValuesAsync(string secretPath);
}