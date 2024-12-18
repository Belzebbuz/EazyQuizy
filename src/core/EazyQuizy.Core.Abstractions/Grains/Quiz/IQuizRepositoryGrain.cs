﻿using EazyQuizy.Common.Grpc.Quiz;
using Orleans.Concurrency;

namespace EazyQuizy.Core.Abstractions.Grains.Quiz;

public interface IQuizRepositoryGrain : IGrainWithGuidKey
{
	Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request);
	
	[ReadOnly]
	Task<GetQuizInfoResponse> GetAsync();

	Task<StatusResponse> AddAsync(AddSingleQuestionRequest request);
	Task<StatusResponse> AddAsync(AddMultipleQuestionRequest request);
	Task<StatusResponse> DeleteAsync();
}