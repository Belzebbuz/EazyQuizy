using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Domain.Entities;
using Throw;
using LobbyQuizInfo = EazyQuizy.Core.Domain.Entities.LobbyQuizInfo;
using LobbySettings = EazyQuizy.Common.Grpc.Lobby.LobbySettings;
using LobbyState = EazyQuizy.Core.Domain.Entities.LobbyState;

namespace EazyQuizy.Core.Grains.Lobbies.Extensions;

public static class LobbyMapExtensions
{
	public static Lobby ToLobby(this CreateLobbyRequest request, Guid id, LobbyQuizInfo quiz)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		var currentUserName = RequestContext.Get(RequestKeys.Name) as string;
		currentUserName.ThrowIfNull();
		var quizId = Guid.Parse(request.QuizId);
		var lobby = new Lobby
		{
			Id = id,
			Quiz = quiz,
			InviteCode = 0,
			OwnerId = currentUserId,
			Settings = new Domain.Entities.LobbySettings()
			{
				TimePerQuestion = request.Settings.TimePerQuestion,
				IsOpen = request.Settings.IsOpen,
				MaxPlayersCount = request.Settings.MaxPlayersCount,
			},
			ChannelKey = Guid.NewGuid(),
			State = LobbyState.Created
		};
		return lobby;
	}

	public static GetLobbyInfoResponse ToResponse(this Lobby lobby, string updateChannel)
	{
		return new GetLobbyInfoResponse()
		{
			Id = lobby.Id.ToString(),
			Settings = new LobbySettings()
			{
				IsOpen = lobby.Settings.IsOpen,
				MaxPlayersCount = lobby.Settings.MaxPlayersCount,
				TimePerQuestion = lobby.Settings.TimePerQuestion
			},
			Players = { lobby.Players.OrderByDescending(x => x.ConnectedAt).Select(MapPlayer) },
			QuizInfo = new EazyQuizy.Common.Grpc.Lobby.LobbyQuizInfo()
			{
				Id = lobby.Quiz.Id.ToString(),
				Name = lobby.Quiz.Name
			},
			GameId = lobby.GameId.ToString(),
			State = (EazyQuizy.Common.Grpc.Lobby.LobbyState)lobby.State,
			UpdateChannel = updateChannel,
			CanInvite = lobby.CanInvite,
		};
	}

	private static PlayerInfo MapPlayer(Player player)
	{
		return new PlayerInfo()
		{
			Id = player.Id,
			IsOwner = player.IsOwner,
			Name = player.Name,
			Invited = player.Invited
		};
	}
}