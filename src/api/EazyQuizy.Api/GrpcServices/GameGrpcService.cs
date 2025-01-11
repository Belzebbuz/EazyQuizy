using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Game;
using Grpc.Core;
using Orleans.Concurrency;

namespace EazyQuizy.Api.GrpcServices;

public class GameGrpcService(IClusterClient client) : GameService.GameServiceBase
{
	public override async Task<GetGameInfoResponse> GetGameInfo(GetGameInfoRequest request, ServerCallContext context)
		=> await client.GetGrain<IGameGrain>(Guid.Parse(request.GameId)).GetInfoAsync();

	public override async Task<StatusResponse> Play(PlayRequest request, ServerCallContext context)
		=> await client.GetGrain<IGameGrain>(Guid.Parse(request.GameId)).PlayAsync();

	public override async Task<StatusResponse> SetAnswer(SetAnswerRequest request, ServerCallContext context)
		=> await client.GetGrain<IGameGrain>(Guid.Parse(request.GameId)).SetAnswerAsync(request);
}