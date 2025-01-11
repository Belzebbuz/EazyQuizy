using System.Text.Json.Serialization;

namespace EazyQuizy.Core.Domain.Entities;

public class Quiz
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
	public required string UserId { get; init; }
	public required string? ImageUrl { get; init; }
	public required DateTime ModifiedAt { get; set; }
	public required List<Question> Questions { get; init; } = [];
	public required HashSet<string> Tags { get; init; } = [];
}

public interface IAnswer;

public interface IAnswer<out T> : IAnswer
{
	public T Value { get; }
}

[JsonDerivedType(typeof(SingleAnswersQuestion), nameof(SingleAnswersQuestion))]
[JsonDerivedType(typeof(MultipleAnswersQuestion), nameof(MultipleAnswersQuestion))]
[JsonDerivedType(typeof(RangeQuestion), nameof(RangeQuestion))]
[JsonDerivedType(typeof(OrderQuestion), nameof(OrderQuestion))]
public abstract class Question
{
	public required Guid Id { get; init; }
	public required string Text { get; init; }
	public required string? ImageUrl { get; init; }
	public required int Order { get; set; }
	public abstract bool IsCorrect(IAnswer answer);
}

public abstract class Question<TAnswer> : Question where TAnswer : IAnswer
{
	public override bool IsCorrect(IAnswer answer)
	{
		if (answer is TAnswer specificAnswer)
			return IsCorrect(specificAnswer);
		return false;
	}

	protected abstract bool IsCorrect(TAnswer answer);
}
public class SingleAnswersQuestion : Question<IAnswer<string>>
{
	public required string CorrectAnswer { get; init; }
	public required HashSet<string> WrongAnswers { get; init; }

	public string[] GetVariants(int randomSeed)
	{
		var random = new Random(randomSeed);
		WrongAnswers.Add(CorrectAnswer);
		var array = WrongAnswers.ToArray();
		random.Shuffle(array);
		return array;
	}

	protected override bool IsCorrect(IAnswer<string> answer)
	{
		return answer.Value == CorrectAnswer;
	}
}

public class MultipleAnswersQuestion : Question<IAnswer<HashSet<string>>>
{
	public required HashSet<string> CorrectAnswers { get; init; }
	public required HashSet<string> WrongAnswers { get; init; }
	public string[] GetVariants(int randomSeed)
	{
		var random = new Random(randomSeed);
		var array = WrongAnswers.Concat(CorrectAnswers).ToArray();
		random.Shuffle(array);
		return array;
	}

	protected override bool IsCorrect(IAnswer<HashSet<string>> answer)
	{
		return CorrectAnswers.SetEquals(answer.Value);
	}
}

public class RangeQuestion : Question<IAnswer<int>>
{
	public required int MinValue { get; init; }
	public required int MaxValue { get; init; }
	public required int CorrectValue { get; init; }
	protected override bool IsCorrect(IAnswer<int> answer)
	{
		return CorrectValue == answer.Value;
	}
}

public class OrderQuestion : Question<IAnswer<List<OrderedValue>>>
{
	public required List<OrderedValue> Values { get; init; } = new();
	public string[] GetVariants(int randomSeed)
	{
		var random = new Random(randomSeed);
		var array = Values.Select(x => x.Value).ToArray();
		random.Shuffle(array);
		return array;
	}

	protected override bool IsCorrect(IAnswer<List<OrderedValue>> answer)
	{
		foreach (var value in answer.Value)
		{
			var correctValue = Values.FirstOrDefault(x => x.Order == value.Order);
			if (correctValue?.Value != value.Value)
				return false;
		}

		return true;
	}
}

public class OrderedValue
{
	public required string Value { get; init; }
	public required int Order { get; init; }
}