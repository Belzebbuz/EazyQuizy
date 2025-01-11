using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Authorize;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.StartGameSagas;

public class InitGameAuthSaga(IGrainFactory factory) : ISaga<CreateGameRequest>
{
	public async Task HandleAsync(CreateGameRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ILobbyGrain>(Guid.Parse(message.LobbyId));
		var lobbyInfo = await grain.GetInfoAsync();
		var authGrain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		var playerIds = lobbyInfo.Players
			.Where(x => !x.IsOwner)
			.Select(x => x.Id).ToHashSet();
		await authGrain.InitPolicyAsync();
		await authGrain.UpdatePolicyAsync(playerIds, new HashSet<string>()
		{
			GrainAccessAction.Update, GrainAccessAction.Read
		});
	}

	public async Task RollbackAsync(CreateGameRequest message, ISagaContext context)
	{
		var authGrain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		await authGrain.ClearPolicyAsync();
	}
}