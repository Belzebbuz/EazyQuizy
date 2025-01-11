using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;

namespace EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;

public interface ISagaOrchestratorGrain : IGrainWithGuidKey
{
	Task<StatusResponse> EvaluateSagaAsync(CreateQuizRequest request);
	Task<StatusResponse> EvaluateSagaAsync(CreateLobbyRequest request);
	Task<StatusResponse> EvaluateSagaAsync(CreateGameRequest request);
}