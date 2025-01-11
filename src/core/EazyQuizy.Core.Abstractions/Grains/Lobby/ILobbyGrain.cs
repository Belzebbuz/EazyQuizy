﻿using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Types;
using Orleans.Concurrency;

namespace EazyQuizy.Core.Abstractions.Grains.Lobby;

public interface ILobbyGrain : IGrainWithGuidKey
{
	public Task<StatusResponse> CreateAsync(CreateLobbyRequest request);
	[ReadOnly]
	Task<GetLobbyInfoResponse> GetInfoAsync();

	Task<StatusResponse> UpdateSettingsAsync(UpdateLobbySettingsRequest request);
	Task<StatusResponse> ConnectAsync();
	Task<StatusResponse> DisconnectAsync();
	Task<StatusResponse> InvitePlayerAsync(InvitePlayerRequest request);
	Task<StatusResponse> RemovePlayerAsync(RemovePlayerRequest request);
	Task DeleteAsync();
	Task<StatusResponse> SetGameIdAsync(Guid gameId);
	Task<StatusResponse> ClearGameIdAsync();
}