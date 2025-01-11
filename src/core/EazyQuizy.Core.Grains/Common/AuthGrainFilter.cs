using System.Net;
using System.Reflection;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Abstractions.Grains.Authorize;

namespace EazyQuizy.Core.Grains.Common;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeGrainAttribute(string actionType) : Attribute
{
	public string ActionType { get; private set; } = actionType;
}

public class AuthGrainFilter(IGrainFactory factory) : IIncomingGrainCallFilter
{
	public async Task Invoke(IIncomingGrainCallContext context)
	{
		var needAuthorize =
			context.ImplementationMethod.GetCustomAttribute<AuthorizeGrainAttribute>();
		if (needAuthorize is not null)
		{
			var authGrain = factory.GetGrain<IAuthorizationGrain>(context.TargetContext.GrainId.GetGuidKey());
			var canHandle = await authGrain.EvaluatePolicyAsync(needAuthorize.ActionType);
			if(!canHandle)
				throw DomainException.Create("Нет доступа", HttpStatusCode.Unauthorized);
		}
		await context.Invoke();
	}
}