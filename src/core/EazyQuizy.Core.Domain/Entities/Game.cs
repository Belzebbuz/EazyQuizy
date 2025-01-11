using Newtonsoft.Json;

namespace EazyQuizy.Core.Domain.Entities;

public class Game
{
	public required Guid Id { get; init; }
	public required Guid QuestionId { get; set; }
	public required GameStatus Status { get; set; }
	public required int TimePerQuestion { get; init; }
	public Dictionary<Guid, List<QuestionResult>> QuestionResults { get; init; } = [];
	public Dictionary<string,GamePlayer> Players { get; init; } = [];
	[JsonIgnore] public List<QuestionResult> CurrentResults => QuestionResults[QuestionId];
	public void UpdateQuestionResults(Guid questionId)
	{
		QuestionResults[questionId] = Players.Values.Select(x => new QuestionResult
		{
			PlayerId = x.Id
		}).ToList();
	}
}
public class GamePlayer 
{
	public required string Id { get; init; }
	public required string Name { get; init; }
	public int TotalPoints { get; set; }
}

public class QuestionResult
{
	public required string PlayerId { get; init; }
	public int? CurrentQuestionPoints { get; set; }
}

public enum GameStatus
{
	Created,
	Started,
	Paused,
	Completed
}