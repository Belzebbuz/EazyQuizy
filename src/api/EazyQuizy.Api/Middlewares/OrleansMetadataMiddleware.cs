using System.Security.Claims;
using EazyQuizy.Core.Abstractions.Consts;

namespace EazyQuizy.Api.Middlewares;

public class OrleansMetadataMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var id = context.User.FindFirstValue("sub");
		var name = context.User.FindFirstValue("name");
		RequestContext.Set(RequestKeys.UserId, id);
		RequestContext.Set(RequestKeys.Name, name);
		await next(context);
	}
}