using System.Net;
using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Extensions;

namespace EazyQuizy.Core.Grains.Games.Extensions;

public static class GameMapExtensions
{
	public static Game ToGame(this GetLobbyInfoResponse lobbyInfo, Guid id, Quiz quiz)
	{
		var firstQuestion = quiz.Questions.OrderBy(x => x.Order)
			.First();
		var game = new Game
		{
			Id = id,
			QuestionId = firstQuestion.Id,
			Status = GameStatus.Created,
			Players = lobbyInfo.Players
				.ToDictionary(x => x.Id, x => new GamePlayer
				{
					Id = x.Id,
					Name = x.Name
				}),
			TimePerQuestion = lobbyInfo.Settings.TimePerQuestion,
		};
		game.UpdateQuestionResults(firstQuestion.Id);
		return game;
	}

	public static GetGameInfoResponse ToInfo(this Game game, Quiz quiz, string updateChanel, string updateTimerChannel)
	{
		var currentQuestion = quiz.Questions.FirstOrDefault(x => x.Id == game.QuestionId)
			.ThrowNotFound().Value;
		return new GetGameInfoResponse()
		{
			GameId = game.Id.ToString(),
			Question = currentQuestion.ToQuestionInfo(game.Id.GetHashCode()),
			Players =
			{
				game.Players.Values
					.OrderByDescending(x => x.TotalPoints)
					.Select(x => x.ToInfo(game.QuestionResults[game.QuestionId]))
			},
			TotalQuestions = quiz.Questions.Count,
			CurrentQuestion = game.QuestionResults.Count,
			UpdateChannel = updateChanel,
			UpdateTimerChannel = updateTimerChannel,
			Status = (GameInfoStatus)game.Status
		};
	}

	private static CurrentQuestionInfo ToQuestionInfo(this Question question, int randomSeed)
	{
		return question switch
		{
			SingleAnswersQuestion singleAnswersQuestion => new CurrentQuestionInfo()
			{
				QuestionType = QuestionType.SingleAnswer,
				SingleAnswerQuestionInfo = new GameSingleAnswerQuestionInfo()
				{
					Id = question.Id.ToString(),
					Text = question.Text,
					ImageUrl = question.ImageUrl,
					Answers = { singleAnswersQuestion.GetVariants(randomSeed) }
				}
			},
			MultipleAnswersQuestion multipleAnswersQuestion => new CurrentQuestionInfo()
			{
				QuestionType = QuestionType.MultipleAnswers,
				MultipleAnswerQuestionInfo = new GameMultipleAnswerQuestionInfo()
				{
					Id = question.Id.ToString(),
					Text = question.Text,
					ImageUrl = question.ImageUrl,
					Answers = { multipleAnswersQuestion.GetVariants(randomSeed) }
				}
			},
			RangeQuestion rangeQuestion => new CurrentQuestionInfo()
			{
				QuestionType = QuestionType.RangeAnswer,
				RangeQuestionInfo = new GameRangeQuestionInfo()
				{
					Id = question.Id.ToString(),
					Text = question.Text,
					ImageUrl = question.ImageUrl,
					MinValue = rangeQuestion.MinValue,
					MaxValue = rangeQuestion.MaxValue
				}
			},
			OrderQuestion orderQuestion => new CurrentQuestionInfo()
			{
				QuestionType = QuestionType.OrderAnswers,
				OrderQuestionInfo = new GameOrderQuestionInfo()
				{
					Id = question.Id.ToString(),
					Text = question.Text,
					ImageUrl = question.ImageUrl,
					Answers = { orderQuestion.GetVariants(randomSeed) }
				}
			},
			_ => throw DomainException.Create("Тип вопроса не поддерживается", HttpStatusCode.InternalServerError)
		};
	}

	private static GamePlayerInfo ToInfo(this GamePlayer player, List<QuestionResult> results)
	{
		var result = results.FirstOrDefault(x => x.PlayerId == player.Id);
		return new GamePlayerInfo()
		{
			CurrentQuestionPoints = result?.CurrentQuestionPoints,
			PlayerId = player.Id,
			PlayerName = player.Name,
			TotalPoints = player.TotalPoints,
			Answered = result?.CurrentQuestionPoints is not null
		};
	}
}