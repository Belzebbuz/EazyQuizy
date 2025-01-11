using System.Net;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Exceptions;
using EazyQuizy.Core.Domain.Entities;
using ErrorOr;
using Google.Protobuf;
using Throw;
using OrderedValue = EazyQuizy.Core.Domain.Entities.OrderedValue;

namespace EazyQuizy.Core.Grains.Quizzes.Extensions;

public static class QuestionMapExtensions
{
	private static readonly Dictionary<Type, QuestionType> TypeMap = new Dictionary<Type, QuestionType>()
	{
		{ typeof(SingleAnswersQuestion), QuestionType.SingleAnswer },
		{ typeof(MultipleAnswersQuestion), QuestionType.MultipleAnswers },
		{ typeof(RangeQuestion), QuestionType.RangeAnswer },
		{ typeof(OrderQuestion), QuestionType.OrderAnswers },
	};
	public static ErrorOr<Question> ToQuestion(this IMessage request, int order)
	{
		return request switch
		{
			AddSingleQuestionRequest req => ToQuestion(req, order),
			AddMultipleQuestionRequest req => ToQuestion(req, order),
			AddOrderQuestionRequest req => ToQuestion(req, order),
			AddRangeQuestionRequest req => ToQuestion(req, order),
			_ => Error.Failure(description: "Этот тип вопроса не поддерживается")
		};
	}
	private static ErrorOr<Question> ToQuestion(this AddSingleQuestionRequest request, int order)
	{
		var wrongAnswers = request.Info.WrongAnswers.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (wrongAnswers.Count != request.Info.WrongAnswers.Count)
			return Error.Validation(description: "Ответы не должны повторяться");
		if(wrongAnswers.Contains(request.Info.CorrectAnswer))
			return Error.Validation(description: "Правильный ответ не может содержаться в наборе неправильных ответов");
		var id = Guid.TryParse(request.Info.QuestionId, out var questionId) ? questionId : Guid.CreateVersion7();
		return new SingleAnswersQuestion
		{
			CorrectAnswer = request.Info.CorrectAnswer,
			WrongAnswers = wrongAnswers,
			Id = id,
			Text = request.Info.Text,
			ImageUrl = request.Info.ImageUrl,
			Order = order
		};
	}
	
	private static ErrorOr<Question> ToQuestion(this AddMultipleQuestionRequest request, int order)
	{
		var wrongAnswers = request.Info.WrongAnswers.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var correctAnswers = request.Info.CorrectAnswers.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (wrongAnswers.Count != request.Info.WrongAnswers.Count)
			return Error.Validation(description: "Ответы не должны повторяться");
		if (correctAnswers.Count != request.Info.CorrectAnswers.Count)
			return Error.Validation(description: "Ответы не должны повторяться");
		if(wrongAnswers.Overlaps(correctAnswers))
			return Error.Validation(description: "Правильный ответ не может содержаться в наборе неправильных ответов");
		var id = Guid.TryParse(request.Info.QuestionId, out var questionId) ? questionId : Guid.CreateVersion7();
		return new MultipleAnswersQuestion
		{
			CorrectAnswers = request.Info.CorrectAnswers.ToHashSet(),
			WrongAnswers = request.Info.WrongAnswers.ToHashSet(),
			Id = id,
			Text = request.Info.Text,
			ImageUrl = request.Info.ImageUrl,
			Order = order
		};
	}
	private static ErrorOr<Question> ToQuestion(this AddOrderQuestionRequest request, int order)
	{
		var uniqueValues = request.Info.Values.Select(x => x.Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if(uniqueValues.Count != request.Info.Values.Count)
			return Error.Validation(description: "Ответы не должны повторяться");
		var id = Guid.TryParse(request.Info.QuestionId, out var questionId) ? questionId : Guid.CreateVersion7();
		return new OrderQuestion
		{
			Values = request.Info.Values.Select(x =>  new OrderedValue
			{
				Value = x.Value,
				Order = x.Order
			}).ToList(),
			Id = id,
			Text = request.Info.Text,
			ImageUrl = request.Info.ImageUrl,
			Order = order,
		};
	}
	private static ErrorOr<Question> ToQuestion(this AddRangeQuestionRequest request, int order)
	{
		if(request.Info.MinValue > request.Info.CorrectValue)
			return Error.Validation(description: "Правильный ответ не может быть меньше минимального значения");
		if(request.Info.MaxValue < request.Info.CorrectValue)
			return Error.Validation(description: "Правильный ответ не может быть больше максимального значения");
		if(request.Info.MaxValue <= request.Info.MinValue)
			return Error.Validation(description: "Минимальное значение не может быть больше либо равно максимальному");
		var id = Guid.TryParse(request.Info.QuestionId, out var questionId) ? questionId : Guid.CreateVersion7();
		return new RangeQuestion
		{
			Id = id,
			Text = request.Info.Text,
			ImageUrl = request.Info.ImageUrl,
			Order = order,
			MinValue = request.Info.MinValue,
			MaxValue = request.Info.MaxValue,
			CorrectValue = request.Info.CorrectValue
		};
	}

	public static GetQuestionInfoResponse ToFullInfo(this Question question)
	{
		if (question is MultipleAnswersQuestion multipleAnswersQuestion)
			return new GetQuestionInfoResponse()
			{
				MultipleQuestionInfo = new MultipleQuestionInfo()
				{
					CorrectAnswers = { multipleAnswersQuestion.CorrectAnswers },
					ImageUrl = multipleAnswersQuestion.ImageUrl,
					WrongAnswers = { multipleAnswersQuestion.WrongAnswers },
					Text = multipleAnswersQuestion.Text,
					QuestionId = multipleAnswersQuestion.Id.ToString()
				}
			};
		if (question is SingleAnswersQuestion singleAnswerQuestion)
			return new GetQuestionInfoResponse()
			{
				SingleQuestionInfo = new SingleQuestionInfo()
				{
					CorrectAnswer = singleAnswerQuestion.CorrectAnswer ,
					ImageUrl = singleAnswerQuestion.ImageUrl,
					WrongAnswers = { singleAnswerQuestion.WrongAnswers },
					Text = singleAnswerQuestion.Text,
					QuestionId = singleAnswerQuestion.Id.ToString()
				}
			};
		if (question is RangeQuestion rangeQuestion)
			return new GetQuestionInfoResponse()
			{
				RangeQuestionInfo = new RangeQuestionInfo()
				{
					CorrectValue =rangeQuestion.CorrectValue,
					ImageUrl = rangeQuestion.ImageUrl,
					MinValue = rangeQuestion.MinValue,
					MaxValue = rangeQuestion.MaxValue,
					Text = rangeQuestion.Text,
					QuestionId = rangeQuestion.Id.ToString()
				}
			};
		if (question is OrderQuestion orderQuestion)
			return new GetQuestionInfoResponse()
			{
				OrderQuestionInfo = new OrderQuestionInfo()
				{
					Values = { orderQuestion.Values.Select(x => new EazyQuizy.Common.Grpc.Types.OrderedValue()
					{
						Order = x.Order,
						Value = x.Value
					}) },
					ImageUrl = orderQuestion.ImageUrl,
					Text = orderQuestion.Text,
					QuestionId = orderQuestion.Id.ToString()
				}
			};
		throw DomainException.Create("Тип вопроса не поддерживается", HttpStatusCode.InternalServerError);
	}
	
	private static QuestionType GetQuestionType(this Question question)
	{
		return TypeMap[question.GetType().ThrowIfNull()];
	}
	public static QuestionShortInfo ToShortInfo(this Question question)
	{
		return new QuestionShortInfo()
		{
			Id = question.Id.ToString(),
			Order = question.Order,
			QuestionType = question.GetQuestionType(),
			Text = question.Text
		};
	}
}