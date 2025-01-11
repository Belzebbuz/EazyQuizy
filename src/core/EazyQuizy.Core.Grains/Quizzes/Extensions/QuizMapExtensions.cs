using EazyQuizy.Common.Extensions;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Domain.Entities;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Throw;

namespace EazyQuizy.Core.Grains.Quizzes.Extensions;

public static class QuizMapExtensions
{
	public static QuizInfo ToInfo(this Quiz quiz)
	{
		return new QuizInfo()
		{
			Id = quiz.Id.ToString(),
			Name = quiz.Name,
			ImageUrl = quiz.ImageUrl,
			Tags = {quiz.Tags},
			ModifiedAt = quiz.ModifiedAt.ToTimestamp(),
			CanCreateLobby = quiz.Questions.Count != 0,
		};
	}
	
	public static ErrorOr<Quiz> ToQuiz(this CreateQuizRequest createRequest, Guid id)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();
		if (createRequest.Name.IsNullOrEmpty())
			return Error.Validation(description: "Имя должно быть заполнено");
		var quiz = new Quiz
		{
			Id = id,
			Name = createRequest.Name,
			UserId = currentUserId,
			ImageUrl = createRequest.ImageUrl,
			ModifiedAt = DateTime.UtcNow,
			Questions = [],
			Tags = createRequest.Tags.ToHashSet()
		};
		return quiz;
	}
}