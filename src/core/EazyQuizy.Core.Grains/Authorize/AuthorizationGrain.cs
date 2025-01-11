using System.Net;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Abstractions.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;
using Throw;

namespace EazyQuizy.Core.Grains.Authorize;

public static class GrainAccessAction
{
	public const string Read = "read";
	public const string Delete = "delete";
	public const string Update = "update";
	public const string StartGame = "start-game";

	public static IReadOnlySet<string> Actions { get; } = new HashSet<string>()
	{
		Read, Delete, Update, StartGame
	};
}

public class AuthGrainPolicy : Dictionary<string, HashSet<string>>
{
	public HashSet<string> GlobalPolicy { get; set; } = [];
}
public class AuthorizationGrain(
	[PersistentState("grain-policies", StorageConstants.RedisStorage)]
	IStorage<AuthGrainPolicy> state, INatsConnection connection, ILogger<StateGrain<AuthGrainPolicy>> logger) 
	: StateGrain<AuthGrainPolicy>(state, connection, logger), IAuthorizationGrain
{
	[ThrowIfNoRecord]
	public Task<bool> EvaluatePolicyAsync(string action)
	{
		if(State.State.GlobalPolicy.Contains(action))
			return Task.FromResult(true);
		if (RequestContext.Get(RequestKeys.UserId) is not string userId)
			return Task.FromResult(false);
		if (!State.State.TryGetValue(userId, out var policy))
			return Task.FromResult(false);
		return Task.FromResult(policy.Contains(action));
	}
	
	
	[ThrowIfNoRecord,StateChanger]
	public Task UpdatePolicyAsync(string userId, IReadOnlySet<string> action)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		if(!State.State.TryGetValue(currentUserId, out var currentUserPolicy))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		if(!currentUserPolicy.Contains(GrainAccessAction.Update))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		State.State[userId] = action.ToHashSet();
		return Task.CompletedTask;
	}

	[ThrowIfNoRecord,StateChanger]
	public Task UpdatePolicyAsync(IReadOnlySet<string> usersIds, IReadOnlySet<string> action)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		if(!State.State.TryGetValue(currentUserId, out var currentUserPolicy))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		if(!currentUserPolicy.Contains(GrainAccessAction.Update))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		foreach (var userId in usersIds)
		{
			State.State[userId] = action.ToHashSet();
		}
		return Task.CompletedTask;
	}

	[ThrowIfNoRecord,StateChanger]
	public Task UpdateGlobalPolicyAsync(IReadOnlySet<string> action)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		if(!State.State.TryGetValue(currentUserId, out var currentUserPolicy))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		if(!currentUserPolicy.Contains(GrainAccessAction.Update))
			throw DomainException.Create("Unauthorized", HttpStatusCode.Unauthorized);
		State.State.GlobalPolicy = action.ToHashSet();
		return Task.CompletedTask;
	}
	[StateChanger]
	public Task InitPolicyAsync()
	{
		State.RecordExists.Throw().IfTrue();
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		State.State[currentUserId] = GrainAccessAction.Actions.ToHashSet();
		return Task.CompletedTask;
	}

	public Task ClearPolicyAsync() => State.ClearStateAsync();
}