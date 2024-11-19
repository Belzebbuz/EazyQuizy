using EazyQuizy.Core.Domain.Common;
using ErrorOr;

namespace EazyQuizy.Core.Domain.Entities;

public sealed class Quiz : AuditableEntity<Guid>
{
	public string Name { get; private set; } = "ErrorName";
	
	public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
	private readonly List<Question> _questions = new();

	public static ErrorOr<Quiz> Create(Guid id, string name)
	{
		var entity = new Quiz()
		{
			Id = id,
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

public abstract class Question : BaseEntity<Guid>
{
	public string Text { get; protected set; } = "ErrorName";
	public string? ImageUrl { get; protected set; }
	public int Order { get; protected set; }
}

public class SingleAnswersQuestion : Question
{
	public string CorrectAnswer { get; private set; } = "ErrorName";
	
	public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	private readonly List<string> _wrongAnswers = [];

	private SingleAnswersQuestion(){}
	public static ErrorOr<SingleAnswersQuestion> Create(
		string text, 
		int order, 
		string answer, 
		IEnumerable<string> wrong)
	{
		var entity = new SingleAnswersQuestion()
		{
			Id = Guid.CreateVersion7(),
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
	private readonly List<string> _correctAnswers = [];
	
	public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	private readonly List<string> _wrongAnswers = [];

	private MultipleAnswersQuestion(){}
	public static ErrorOr<MultipleAnswersQuestion> Create(
		string text, 
		int order, 
		IEnumerable<string> answers, 
		IEnumerable<string> wrong)
	{
		var entity = new MultipleAnswersQuestion()
		{
			Id = Guid.CreateVersion7(),
			Order = order,
			Text = text,
		};
		entity._wrongAnswers.AddRange(wrong);
		entity._correctAnswers.AddRange(answers);
		return entity;
	}
}