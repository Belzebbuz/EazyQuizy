using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.StartGameSagas;

public class SetGameIdSaga(IGrainFactory factory) : ISaga<CreateGameRequest>
{
	public async Task HandleAsync(CreateGameRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ILobbyGrain>(Guid.Parse(message.LobbyId));
		await grain.SetGameIdAsync(context.SagaCompositionId);
	}

	public async Task RollbackAsync(CreateGameRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ILobbyGrain>(Guid.Parse(message.LobbyId));
		await grain.ClearGameIdAsync();
	}
}