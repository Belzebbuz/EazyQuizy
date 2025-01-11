using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Authorize;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.CreateLobbySagas;

public class InitLobbyAuthSaga(IGrainFactory factory) : ISaga<CreateLobbyRequest>
{
	public async Task HandleAsync(CreateLobbyRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		await grain.InitPolicyAsync();
	}

	public async Task RollbackAsync(CreateLobbyRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		await grain.ClearPolicyAsync();
	}
}