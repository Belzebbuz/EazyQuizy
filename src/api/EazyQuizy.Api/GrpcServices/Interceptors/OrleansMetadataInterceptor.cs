using System.Security.Claims;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace EazyQuizy.Api.GrpcServices.Interceptors;

public class OrleansMetadataInterceptor : Interceptor
{
	public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		var httpContext = context.GetHttpContext();
		var id = httpContext.User.FindFirstValue("sub");
		var name = httpContext.User.FindFirstValue("name");
		RequestContext.Set("userId", id);
		RequestContext.Set("userName", name);
		return continuation(request, context);
	}
}