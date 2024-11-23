using ErrorOr;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace EazyQuizy.Core.Domain.Entities;

public sealed class Quiz
{
	public string Name { get; private set; } = string.Empty;
	public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
	private List<Question> _questions = [];

	public static ErrorOr<Quiz> Create(string name)
	{
		if (string.IsNullOrEmpty(name))
			return Error.Validation();
		var entity = new Quiz()
		{
			Name = name
		};
		return entity;
	}

	public ErrorOr<Success> AddRange(IEnumerable<Question> questions)
	{
		_questions.AddRange(questions);
		return Result.Success;
	}
}

public abstract class Question
{
	[BsonGuidRepresentation(GuidRepresentation.Standard)]
	public Guid Id { get; private set; } = Guid.CreateVersion7();
	public string Text { get; protected init; } = string.Empty;
	public string? ImageUrl { get; protected set; }
	public int Order { get; protected init; }
}

public class SingleAnswersQuestion : Question
{
	public string CorrectAnswer { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	private List<string> _wrongAnswers = [];

	private SingleAnswersQuestion(){}
	public static ErrorOr<SingleAnswersQuestion> Create(
		string text, 
		int order, 
		string answer, 
		IEnumerable<string> wrong)
	{
		var entity = new SingleAnswersQuestion()
		{
			Order = order,
			Text = text,
			CorrectAnswer = answer
		};
		entity._wrongAnswers.AddRange(wrong);
		return entity;
	}
}

public class MultipleAnswersQuestion : Question
{
	public IReadOnlyCollection<string> CorrectAnswers => _correctAnswers.AsReadOnly();
	private List<string> _correctAnswers = [];
	
	public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	private List<string> _wrongAnswers = [];

	private MultipleAnswersQuestion(){}
	public static ErrorOr<MultipleAnswersQuestion> Create(
		string text, 
		int order, 
		IEnumerable<string> answers, 
		IEnumerable<string> wrong)
	{
		var entity = new MultipleAnswersQuestion()
		{
			Order = order,
			Text = text,
		};
		entity._wrongAnswers.AddRange(wrong);
		entity._correctAnswers.AddRange(answers);
		return entity;
	}
}