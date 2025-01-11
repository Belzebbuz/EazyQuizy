using System.Net;
using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Abstractions.Grains.Game;
using EazyQuizy.Core.Abstractions.Grains.Lobby;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using EazyQuizy.Core.Grains.Extensions;
using EazyQuizy.Core.Grains.Games.Extensions;
using Marten;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;
using Throw;

namespace EazyQuizy.Core.Grains.Games;

public class GameGrain(
	[PersistentState("game-state", StorageConstants.RedisStorage)]
	IStorage<Game> state, 
	[PersistentState("quiz-game-snapshot", StorageConstants.RedisStorage)]
	IStorage<Quiz> quizState,
	IDocumentStore documentStore,
	INatsConnection connection, 
	ILogger<StateGrain<Game>> logger) 
	: StateGrain<Game>(state, connection, logger), IGameGrain
{
	[StateChanger]
	public async Task<StatusResponse> CreateAsync(CreateGameRequest request)
	{
		var lobbyInfo = await GrainFactory.GetGrain<ILobbyGrain>(Guid.Parse(request.LobbyId))
			.GetInfoAsync();
		await using var session = documentStore.QuerySession();
		var quiz = await session.LoadAsync<Quiz>(Guid.Parse(lobbyInfo.QuizInfo.Id));
		quiz.ThrowNotFound();
		if(quiz.Questions.Count == 0)
			throw DomainException.Create("Невозможно создать игру, т.к. в квизе нет вопросов", HttpStatusCode.BadRequest);
		quizState.State = quiz;
		await quizState.WriteStateAsync();
		State.State = lobbyInfo.ToGame(this.GetPrimaryKey(), quizState.State);
		return StatusResponseHelpers.Success();
	}

	public async Task<StatusResponse> DeleteAsync()
	{
		await State.ClearStateAsync();
		await quizState.ClearStateAsync();
		return StatusResponseHelpers.Success();
	}

	[ThrowIfNoRecord]
	[AuthorizeGrain(GrainAccessAction.Read)]
	public Task<GetGameInfoResponse> GetInfoAsync()
	{
		return Task.FromResult(State.State.ToInfo(quizState.State, GetUpdateChannel(),
			GetTimerUpdateChannel()));
	}

	[ThrowIfNoRecord, StateChanger]
	[AuthorizeGrain(GrainAccessAction.StartGame)]
	public async Task<StatusResponse> PlayAsync()
	{
		if (State.State.Status == GameStatus.Completed)
			return StatusResponseHelpers.Success();
		State.State.Status = GameStatus.Started;
		await GrainFactory.GetGrain<IGameTimerGrain>(this.GetPrimaryKey())
			.StartAsync(State.State.TimePerQuestion, GetTimerUpdateChannel());
		return StatusResponseHelpers.Success();
	}

	[ThrowIfNoRecord,StateChanger]
	public Task<bool> NextQuestionAsync()
	{
		if(State.State.Status != GameStatus.Started)
			throw DomainException.Create("Игра еще не началась", HttpStatusCode.InternalServerError);
		var currentQuestion = quizState.State.Questions.SingleOrDefault(x => x.Id == State.State.QuestionId);
		currentQuestion.ThrowNotFound();
		var nextQuestion = quizState.State.Questions.SingleOrDefault(x => x.Order == currentQuestion.Order + 1);
		if (nextQuestion is null)
		{
			State.State.Status = GameStatus.Completed;
			return Task.FromResult(true);
		}
		State.State.QuestionId = nextQuestion.Id;
		State.State.UpdateQuestionResults(nextQuestion.Id);
		return Task.FromResult(false);
	}

	[ThrowIfNoRecord,StateChanger]
	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> SetAnswerAsync(SetAnswerRequest request)
	{
		if(State.State.Status != GameStatus.Started)
			throw DomainException.Create("Игра еще не началась", HttpStatusCode.BadRequest);
		
		var timerGrain = GrainFactory.GetGrain<IGameTimerGrain>(this.GetPrimaryKey());
		var currentTimerValue = await  timerGrain.GetCurrentAsync();
		if(!currentTimerValue.HasValue)
			throw DomainException.Create("Отсчет не идет", HttpStatusCode.BadRequest);
		
		var percent = (double)currentTimerValue / State.State.TimePerQuestion;
		var points = (int)Math.Round(percent * 1000);
		var playerResult = State.State.CurrentResults
			.SingleOrDefault(x => x.PlayerId == GetPlayerId());
		if(playerResult is null)
			throw DomainException.Create("Игрок не найден", HttpStatusCode.BadRequest);
		
		if(playerResult.CurrentQuestionPoints.HasValue)
			throw DomainException.Create("Игрок уже подтвердил ответ", HttpStatusCode.BadRequest);
		
		var question = quizState.State.Questions
			.SingleOrDefault(x => x.Id == State.State.QuestionId)
			.ThrowNotFound();
		var isCorrect = question.Value.IsCorrect(request.ToAnswer());
		var resultPoints = isCorrect ? points : 0;
		playerResult.CurrentQuestionPoints = resultPoints;
		State.State.Players[GetPlayerId()].TotalPoints += resultPoints;
		
		if (State.State.CurrentResults.Any(x => !x.CurrentQuestionPoints.HasValue))
			return StatusResponseHelpers.Success();

		var isQuestionsDone = await NextQuestionAsync();
		if (!isQuestionsDone)
		{
			await timerGrain.StartAsync(State.State.TimePerQuestion, GetTimerUpdateChannel());
			return StatusResponseHelpers.Success();
		}
		
		await timerGrain.StopAsync();
		State.State.Status = GameStatus.Completed;
		return StatusResponseHelpers.Success();
	}
	private string GetPlayerId()
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		return currentUserId.ThrowIfNull();
	}
	private string GetTimerUpdateChannel() => $"timer.update.{this.GetPrimaryKey()}";
}