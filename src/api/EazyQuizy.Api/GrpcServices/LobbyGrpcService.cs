using EazyQuizy.Common.Extensions;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class LobbyGrpcService(IClusterClient client) : LobbyService.LobbyServiceBase
{
	public override Task<StatusResponse> Create(CreateLobbyRequest request, ServerCallContext context)
		=> client.GetGrain<ISagaOrchestratorGrain>(Guid.CreateVersion7())
			.EvaluateSagaAsync(request);
	
	[AllowAnonymous]
	public override Task<GetLobbyInfoResponse> GetInfo(GetLobbyInfoRequest request, ServerCallContext context)
		=> client.GetGrain<ILobbyGrain>(Guid.Parse(request.Id))
			.GetInfoAsync();
	
	public override Task<StatusResponse> UpdateSettings(UpdateLobbySettingsRequest request, ServerCallContext context)
		=> client.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.UpdateSettingsAsync(request);

	[AllowAnonymous]
	public override Task<StatusResponse> ConnectPlayer(ConnectPlayerRequest request, ServerCallContext context)
	{
		var userId = RequestContext.Get(RequestKeys.UserId) as string;
		if(userId.IsNullOrEmpty())
			userId = Guid.NewGuid().ToString();
		RequestContext.Set(RequestKeys.Name, request.PlayerName);
		RequestContext.Set(RequestKeys.UserId, userId);
		return client.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.ConnectAsync();
	}

	[AllowAnonymous]
	public override Task<StatusResponse> DisconnectPlayer(DisconnectPlayerRequest request, ServerCallContext context) =>
		client.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.DisconnectAsync();

	public override Task<StatusResponse> InvitePlayer(InvitePlayerRequest request, ServerCallContext context)
		=> client.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.InvitePlayerAsync(request);
	public override Task<StatusResponse> RemovePlayer(RemovePlayerRequest request, ServerCallContext context)
		=> client.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.RemovePlayerAsync(request);

	public override Task<StatusResponse> StartGame(CreateGameRequest request, ServerCallContext context)
		=> client.GetGrain<ISagaOrchestratorGrain>(Guid.CreateVersion7())
			.EvaluateSagaAsync(request);
}