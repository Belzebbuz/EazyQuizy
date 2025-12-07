using System.Net;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using EazyQuizy.Core.Grains.Extensions;
using EazyQuizy.Core.Grains.Lobbies.Extensions;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;
using Throw;
using LobbyQuizInfo = EazyQuizy.Core.Domain.Entities.LobbyQuizInfo;
using LobbyState = EazyQuizy.Core.Domain.Entities.LobbyState;

namespace EazyQuizy.Core.Grains.Lobbies;

public class LobbyGrain([PersistentState("lobby-state", StorageConstants.RedisStorage)]
	IStorage<Lobby> state, INatsConnection connection, ILogger<StateGrain<Lobby>> logger) 
	: StateGrain<Lobby>(state, connection, logger), ILobbyGrain
{

	[StateChanger]
	public async Task<StatusResponse> CreateAsync(CreateLobbyRequest request)
	{
		if (State.RecordExists)
			throw DomainException.Create("Лобби уже создано", HttpStatusCode.BadRequest);
		var quizId = Guid.Parse(request.QuizId);
		var quiz = await GrainFactory.GetGrain<IQuizRepositoryGrain>(quizId)
			.GetInfoAsync(new GetQuizInfoRequest());
		if(quiz is null)
			throw DomainException.Create("Такого квиза не существует", HttpStatusCode.NotFound);
		if(quiz.Questions.Count == 0)
			throw DomainException.Create("Невозможно создать лобби, тк в квизе нет вопросов", HttpStatusCode.NotFound);
		var quizInfo = new LobbyQuizInfo
		{
			Id = quizId,
			Name = quiz.Quiz.Name
		};
		State.State = request.ToLobby(this.GetPrimaryKey(), quizInfo);
		return new StatusResponse()
		{
			OperationId = this.GetPrimaryKey().ToString(),
			Succeeded = true
		};
	}

	[ThrowIfNoRecord]
	public Task<GetLobbyInfoResponse> GetInfoAsync()
	{
		var response = State.State.ToResponse(GetUpdateChannel());
		return Task.FromResult(response);
	}

	public Task<GetLobbyInfoResponse> GetInfoAsync(string key)
	{
		var response = State.State.ToResponse(GetUpdateChannel());
		return Task.FromResult(response);
	}

	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> UpdateSettingsAsync(UpdateLobbySettingsRequest request)
	{
		if (State.State.State != LobbyState.Created)
		{
			var res = StatusResponseHelpers.Error("Запрещено менять настройки в текущем статусе");
			return Task.FromResult(res);
		}

		State.State.Settings.IsOpen = request.Settings.IsOpen;
		State.State.Settings.MaxPlayersCount = request.Settings.MaxPlayersCount;
		State.State.Settings.TimePerQuestion = request.Settings.TimePerQuestion;
		return Task.FromResult(StatusResponseHelpers.Success());
	}

	[ThrowIfNoRecord, StateChanger]
	public Task<StatusResponse> ConnectAsync()
	{
		if (State.State.State != LobbyState.Created)
		{
			var res = StatusResponseHelpers.Error("Невозможно менять настройки в текущем статусе");
			return Task.FromResult(res);
		}

		var playerAdded = State.State.Players.Exists(x => x.Id == GetPlayerId());
		if(playerAdded)
			return Task.FromResult(StatusResponseHelpers.Success("playerId", GetPlayerId()));
		var player = CreatePlayer(DateTime.UtcNow);
		var invitePlayer = player.IsOwner || (State.State.CanInvite && State.State.Settings.IsOpen);
		player.Invited = invitePlayer;
		State.State.Players.Add(player);
		var response = StatusResponseHelpers.Success("playerId", GetPlayerId());
		return Task.FromResult(response);
	}

	[ThrowIfNoRecord, StateChanger]
	public Task<StatusResponse> DisconnectAsync()
	{
		State.State.Players.RemoveAll(x => x.Id == GetPlayerId());
		return Task.FromResult(StatusResponseHelpers.Success());
	}

	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> InvitePlayerAsync(InvitePlayerRequest request)
	{
		if (State.State.State != LobbyState.Created)
		{
			var res = StatusResponseHelpers.Error("Невозможно менять настройки в текущем статусе");
			return Task.FromResult(res);
		}
		if (!State.State.CanInvite)
		{
			var res = StatusResponseHelpers.Error("Сейчас невозможно добавить игроков, измените в настройках максимальное кол-во игроков");
			return Task.FromResult(res);
		}

		var player = State.State.Players.SingleOrDefault(x => x.Id == request.PlayerId);
		if (player is null)
		{
			var res = StatusResponseHelpers.Error("Игрок не найден");
			return Task.FromResult(res);
		}
		player.Invited = true;
		return Task.FromResult(StatusResponseHelpers.Success());
	}
	
	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> RemovePlayerAsync(RemovePlayerRequest request)
	{
		State.State.Players.RemoveAll(x => x.Id == request.PlayerId);
		return Task.FromResult(StatusResponseHelpers.Success());
	}
	public Task DeleteAsync() => State.ClearStateAsync();
	
	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> SetGameIdAsync(Guid gameId)
	{
		if (State.State.State != LobbyState.Created)
			throw DomainException.Create("Игра уже начата", HttpStatusCode.BadRequest); 
		State.State.GameId = gameId;
		State.State.State = LobbyState.Started;
		return Task.FromResult(StatusResponseHelpers.Success());
	}
	
	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> ClearGameIdAsync()
	{
		if (State.State.State != LobbyState.Created)
			return Task.FromResult(StatusResponseHelpers.Error("Игра уже начата"));
		State.State.GameId = null;
		return Task.FromResult(StatusResponseHelpers.Success());
	}
	
	private Player CreatePlayer(DateTime connectedAt)
	{
		var currentUserName = RequestContext.Get(RequestKeys.Name) as string;
		currentUserName.ThrowIfNull();
		var playerId = GetPlayerId();
		return new Player
		{
			Id = playerId,
			Name = currentUserName,
			IsOwner = State.State.OwnerId == playerId, 
			ConnectedAt = connectedAt,
			Invited = false
		};
	}

	private string GetPlayerId()
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		return currentUserId.ThrowIfNull();
	}
	

	protected override string GetUpdateChannel() => $"lobby.update.{State.State.ChannelKey}";
}