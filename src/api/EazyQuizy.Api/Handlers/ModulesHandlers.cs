using EazyQuizy.Api.Protos.Modules;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace EazyQuizy.Api.Handlers;

[Authorize]
public class ModulesHandlers : ModuleService.ModuleServiceBase
{
	public override Task<CreateModuleResponse> Create(CreateModuleRequest request, ServerCallContext context)
	{
		return Task.FromResult(new CreateModuleResponse()
		{
			Id = Guid.NewGuid().ToString()
		});
	}
}