using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Types;
using Orleans.Concurrency;

namespace EazyQuizy.Core.Abstractions.Grains.Game;

public interface IGameGrain : IGrainWithGuidKey
{
	Task<StatusResponse> CreateAsync(CreateGameRequest request);
	Task<StatusResponse> DeleteAsync();
	
	[ReadOnly]
	Task<GetGameInfoResponse> GetInfoAsync();

	Task<StatusResponse> PlayAsync();
	Task<bool> NextQuestionAsync();
	Task<StatusResponse> SetAnswerAsync(SetAnswerRequest request);
}