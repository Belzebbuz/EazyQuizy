using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using Orleans.Concurrency;

namespace EazyQuizy.Core.Abstractions.Grains.Quiz;

public interface IQuizRepositoryGrain : IGrainWithGuidKey
{
	Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request);
	Task<StatusResponse> DeleteAsync();
	Task<StatusResponse> AddOrUpdateAsync(AddSingleQuestionRequest request);
	Task<StatusResponse> AddOrUpdateAsync(AddMultipleQuestionRequest request);
	Task<StatusResponse> AddOrUpdateAsync(AddRangeQuestionRequest request);
	Task<StatusResponse> AddOrUpdateAsync(AddOrderQuestionRequest request);
	
	[ReadOnly]
	Task<GetQuizInfoResponse> GetInfoAsync(GetQuizInfoRequest request);
	
	[ReadOnly]
	Task<SearchUserQuizResponse> SearchAsync(SearchUserQuizRequest request);

	Task<StatusResponse> SetQuestNewOrderAsync(SetQuestionNewOrderRequest request);
	
	[ReadOnly]
	Task<GetQuestionInfoResponse> GetQuestionInfoAsync(GetQuestionInfoRequest request);

	Task<StatusResponse> DeleteQuestionAsync(DeleteQuestionRequest request);
}