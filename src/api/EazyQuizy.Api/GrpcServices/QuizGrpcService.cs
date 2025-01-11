using EazyQuizy.Api.Infrastructure.Gpt;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class QuizGrpcService(IClusterClient client, IWrongAnswersService service) : QuizService.QuizServiceBase
{
	public override Task<StatusResponse> Create(CreateQuizRequest request, ServerCallContext context)
		=> client.GetGrain<ISagaOrchestratorGrain>(Guid.CreateVersion7())
			.EvaluateSagaAsync(request);

	public override Task<SearchUserQuizResponse> SearchUserQuiz(SearchUserQuizRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.CreateVersion7())
			.SearchAsync(request);
	public override Task<StatusResponse> AddSingleQuestion(AddSingleQuestionRequest request,
		ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddOrUpdateAsync(request);
	public override Task<StatusResponse> AddMultipleQuestion(AddMultipleQuestionRequest request,
		ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddOrUpdateAsync(request);

	public override Task<StatusResponse> AddRangeQuestion(AddRangeQuestionRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddOrUpdateAsync(request);

	public override Task<StatusResponse> AddOrderQuestion(AddOrderQuestionRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.AddOrUpdateAsync(request);

	public override Task<StatusResponse> SetQuestionNewOrder(SetQuestionNewOrderRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.SetQuestNewOrderAsync(request);


	public override Task<GetQuizInfoResponse> GetQuizInfo(GetQuizInfoRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.Id))
			.GetInfoAsync(request);

	public override Task<GetQuestionInfoResponse> GetQuestionInfo(GetQuestionInfoRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.GetQuestionInfoAsync(request);

	public override Task<StatusResponse> DeleteQuestion(DeleteQuestionRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.DeleteQuestionAsync(request);

	public override Task<StatusResponse> DeleteQuiz(DeleteQuizRequest request, ServerCallContext context)
		=> client.GetGrain<IQuizRepositoryGrain>(Guid.Parse(request.QuizId))
			.DeleteAsync();
	public override async Task<GenerateWrongAnswersResponse> GenerateWrongAnswers(GenerateWrongAnswersRequest request, ServerCallContext context)
	{
		var result = await service.GenerateAsync(request);
		return new GenerateWrongAnswersResponse()
		{
			Answers = { result.IsError ? [] : result.Value },
			Error = result.IsError ? result.FirstError.Description : null,
			Success = !result.IsError
		};
	}
}