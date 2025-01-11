using EazyQuizy.Common.Grpc.Auth;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Grains.Authorize;
using Grpc.Core;

namespace EazyQuizy.Api.GrpcServices;

public class AuthGrpcService(IClusterClient client) : AuthService.AuthServiceBase
{
	public override async Task<IsAuthorizedResponse> IsAuthorized(IsAuthorizedRequest request, ServerCallContext context)
	{
		var authenticatedId = RequestContext.Get(RequestKeys.UserId) as string;
		var userId = authenticatedId ?? context.RequestHeaders.FirstOrDefault(x => x.Key == "x-player-id")?.Value;
		RequestContext.Set(RequestKeys.UserId, userId);
		return new IsAuthorizedResponse()
		{
			Authorized = await client.GetGrain<IAuthorizationGrain>(Guid.Parse(request.GrainId)).EvaluatePolicyAsync(request.Action)
		};
	}
}