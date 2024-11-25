using EazyQuizy.Common.Grpc.Quiz;

namespace EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;

public interface ISagaOrchestratorGrain : IGrainWithGuidKey
{
	Task<StatusResponse> EvaluateSagaAsync(CreateQuizRequest request);
}