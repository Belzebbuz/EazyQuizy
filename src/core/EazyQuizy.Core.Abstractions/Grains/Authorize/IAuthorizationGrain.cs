namespace EazyQuizy.Core.Abstractions.Grains.Authorize;

public interface IAuthorizationGrain : IGrainWithGuidKey
{
	Task<bool> EvaluatePolicyAsync(string action);
	Task UpdatePolicyAsync(string userId, [Immutable] IReadOnlySet<string> action);
	/// <summary>
	/// Задает политики грейн Id для набора пользователей
	/// </summary>
	/// <param name="usersIds">Набор пользователей</param>
	/// <param name="action">Действия</param>
	/// <returns>Задача</returns>
	Task UpdatePolicyAsync([Immutable] IReadOnlySet<string> usersIds, [Immutable] IReadOnlySet<string> action);
	Task UpdateGlobalPolicyAsync([Immutable] IReadOnlySet<string> action);
	Task InitPolicyAsync();
	Task ClearPolicyAsync();
}