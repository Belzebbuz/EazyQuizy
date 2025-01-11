using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.CreateLobbySagas;

public class CreateLobbySaga(IGrainFactory factory) : ISaga<CreateLobbyRequest>
{
	public async Task HandleAsync(CreateLobbyRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ILobbyGrain>(context.SagaCompositionId);
		await grain.CreateAsync(message);
	}

	public async Task RollbackAsync(CreateLobbyRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ILobbyGrain>(context.SagaCompositionId);
		await grain.DeleteAsync();
	}
}