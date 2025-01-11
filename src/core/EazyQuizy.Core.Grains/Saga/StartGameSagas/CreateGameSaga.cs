using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Game;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.StartGameSagas;

public class CreateGameSaga(IGrainFactory factory) : ISaga<CreateGameRequest>
{
	public async Task HandleAsync(CreateGameRequest message, ISagaContext context)
	{
		var gameGrain = factory.GetGrain<IGameGrain>(context.SagaCompositionId);
		await gameGrain.CreateAsync(message);
	}

	public async Task RollbackAsync(CreateGameRequest message, ISagaContext context)
	{
		var gameGrain = factory.GetGrain<IGameGrain>(context.SagaCompositionId);
		await gameGrain.DeleteAsync();
	}
}