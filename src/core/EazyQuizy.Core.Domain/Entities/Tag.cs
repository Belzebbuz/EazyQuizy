namespace EazyQuizy.Core.Domain.Entities;

public class Tag
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required HashSet<Guid> QuizIds { get; set; }
}