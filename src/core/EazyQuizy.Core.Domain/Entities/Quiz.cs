using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ErrorOr;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace EazyQuizy.Core.Domain.Entities;

public sealed record Quiz
{
	[JsonInclude] public Guid Id { get; private set; }
	[JsonInclude] public string Name { get; private set; } = string.Empty;
	[JsonInclude] public string? ImageUrl { get; private set; }
	[JsonIgnore] public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
	[JsonInclude] private readonly List<Question> _questions = [];
	[JsonIgnore] public IReadOnlySet<string> Tags => new ReadOnlySet<string>(_tags);
	[JsonInclude] private readonly HashSet<string> _tags = [];
	[JsonConstructor]
	private Quiz(List<Question> _questions, HashSet<string> _tags)
	{
		this._questions = _questions;
		this._tags = _tags;
	}
	private Quiz(){}
	public static ErrorOr<Quiz> Create(Guid id, string name, string? imageUrl = null)
	{
		if (string.IsNullOrEmpty(name))
			return Error.Validation();
		var entity = new Quiz()
		{
			Id = id,
			Name = name,
			ImageUrl = imageUrl
		};
		return entity;
	}
	public ErrorOr<Success> Update(string name, string? imageUrl = null)
	{
		if (string.IsNullOrEmpty(name))
			return Error.Validation();
		Name = name;
		ImageUrl = imageUrl;
		return Result.Success;
	}

	public ErrorOr<Success> AddTags(IReadOnlySet<string> tags)
	{
		foreach (var tagName in tags)
		{
			_tags.Add(tagName);
		}

		return Result.Success;
	}
	public ErrorOr<Success> AddRange(IEnumerable<Question> questions)
	{
		_questions.AddRange(questions);
		return Result.Success;
	}
}


[JsonDerivedType(typeof(SingleAnswersQuestion),nameof(SingleAnswersQuestion))]
[JsonDerivedType(typeof(MultipleAnswersQuestion),nameof(MultipleAnswersQuestion))]
public class Question
{
	[JsonInclude] public Guid Id { get; private set; } = Guid.CreateVersion7();
	[JsonInclude] public string Text { get; protected init; } = string.Empty;
	[JsonInclude] public string? ImageUrl { get; protected set; }
	[JsonInclude] public  int Order { get; protected init; }
}

public class SingleAnswersQuestion : Question
{
	[JsonInclude] public string CorrectAnswer { get; private set; } = string.Empty;
	
	[JsonIgnore] public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	[JsonInclude] private readonly List<string> _wrongAnswers = [];

	[JsonConstructor]
	private SingleAnswersQuestion(List<string> _wrongAnswers)
	{
		this._wrongAnswers = _wrongAnswers;
	}
	private SingleAnswersQuestion()
	{
	}
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
	[JsonIgnore] public IReadOnlyCollection<string> CorrectAnswers => _correctAnswers.AsReadOnly();
	[JsonInclude] private readonly List<string> _correctAnswers = [];
	
	[JsonIgnore] public IReadOnlyCollection<string> WrongAnswers => _wrongAnswers.AsReadOnly();
	[JsonInclude] private readonly List<string> _wrongAnswers = [];

	[JsonConstructor]
	private MultipleAnswersQuestion(List<string> _correctAnswers, List<string> _wrongAnswers)
	{
		this._correctAnswers = _correctAnswers;
		this._wrongAnswers = _wrongAnswers;
	}

	private MultipleAnswersQuestion()
	{
	}
	
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