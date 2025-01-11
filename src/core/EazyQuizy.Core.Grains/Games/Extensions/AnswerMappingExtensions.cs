using EazyQuizy.Common.Extensions;
using EazyQuizy.Common.Grpc.Game;
using EazyQuizy.Core.Domain.Entities;

namespace EazyQuizy.Core.Grains.Games.Extensions;

public record SingleAnswer(string Value) : IAnswer<string>;
public record RangeAnswer(int Value) : IAnswer<int>;
public record OrderAnswer(List<OrderedValue> Value) : IAnswer<List<OrderedValue>>;
public record MultipleAnswer(HashSet<string> Value) : IAnswer<HashSet<string>>;

public static class AnswerMappingExtensions
{
	public static IAnswer ToAnswer(this SetAnswerRequest request)
	{
		if (request.MultipleAnswer.Count != 0)
			return new MultipleAnswer(request.MultipleAnswer.ToHashSet());
		if (!request.SingleAnswer.IsNullOrEmpty())
			return new SingleAnswer(request.SingleAnswer);
		if (request.RangeAnswer.HasValue)
			return new RangeAnswer(request.RangeAnswer.Value);
		if (request.OrderedAnswer.Count != 0)
			return new OrderAnswer(request.OrderedAnswer.Select(x => new OrderedValue
			{
				Value = x.Value,
				Order = x.Order
			}).ToList());

		throw new ArgumentOutOfRangeException();
	}
}