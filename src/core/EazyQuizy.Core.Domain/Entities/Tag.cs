using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ErrorOr;

namespace EazyQuizy.Core.Domain.Entities;

public class Tag
{
	[JsonInclude] public Guid Id { get; private set; }
	[JsonInclude] public string Name { get; private set; } = string.Empty;
	[JsonIgnore] public IReadOnlySet<Guid> QuizIds => new ReadOnlySet<Guid>(_quizIds);
	[JsonInclude] private readonly HashSet<Guid> _quizIds = [];

	[JsonConstructor]
	private Tag(HashSet<Guid> _quizIds)
	{
		this._quizIds = _quizIds;
	}
	private Tag(){}

	public static ErrorOr<Tag> Create(Guid id, string name, IReadOnlySet<Guid> quizIds)
	{
		if (string.IsNullOrEmpty(name))
			return Error.Validation();
		var tag = new Tag()
		{
			Id = id,
			Name = name,
		};
		foreach (var quizId in quizIds)
		{
			tag._quizIds.Add(quizId);
		}

		return tag;

	}

	public void AddQuizIds(HashSet<Guid> quizIds)
	{
		if(quizIds.Count == 0)
			return;
		foreach (var quizId in quizIds)
		{
			_quizIds.Add(quizId);
		}
	}

	public void RemoveQuizId(Guid quizId)
	{
		_quizIds.Remove(quizId);
	}
}