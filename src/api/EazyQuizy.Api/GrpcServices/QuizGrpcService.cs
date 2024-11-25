using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class QuizGrpcService(IClusterClient client) : QuizService.QuizServiceBase
{
	public override Task<StatusResponse> Create(CreateQuizRequest request, ServerCallContext context)
		=> client.GetGrain<ISagaOrchestratorGrain>(Guid.CreateVersion7()).EvaluateSagaAsync(request);

	public override Task<GetQuizInfoResponse> GetInfo(GetQuizInfoRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.Id))
			.GetAsync();

	public override Task<StatusResponse> AddSingleQuestion(AddSingleQuestionRequest request,
		ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddAsync(request);
	public override Task<StatusResponse> AddMultipleQuestion(AddMultipleQuestionRequest request,
		ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddAsync(request);
}