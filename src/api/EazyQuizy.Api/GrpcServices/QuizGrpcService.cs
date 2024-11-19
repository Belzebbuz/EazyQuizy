using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class QuizGrpcService(IClusterClient client) : QuizService.QuizServiceBase
{
	public override Task<CreateQuizResponse> Create(CreateQuizRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizGrain>(Guid.CreateVersion7()).CreateAsync(request);

	public override Task<GetQuizInfoResponse> GetInfo(GetQuizInfoRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizGrain>(Guid.Parse(request.Id))
			.GetAsync();
}