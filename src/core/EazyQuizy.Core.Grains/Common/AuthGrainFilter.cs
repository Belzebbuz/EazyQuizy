using System.Reflection;
using EazyQuizy.Core.Grains.Authorize;
using Serilog;
using Throw;

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
			canHandle.Throw("Unauthorized").IfFalse();
		}
		await context.Invoke();
	}
}