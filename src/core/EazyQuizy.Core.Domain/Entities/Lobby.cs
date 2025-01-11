using System.Text.Json.Serialization;

namespace EazyQuizy.Core.Domain.Entities;

public class Lobby
{
	public required Guid Id { get; init; }
	public required LobbyQuizInfo Quiz { get; set; }
	public required Guid ChannelKey { get; init; }
	public required int InviteCode { get; init; }
	public required string OwnerId { get; init; }
	public required LobbySettings Settings { get; init; }
	public required LobbyState State { get; set; }
	public List<Player> Players { get; init; } = [];
	public Guid? GameId { get; set; }
	[JsonIgnore] public bool CanInvite => State == LobbyState.Created 
	                                       && Players.Count(x => x.Invited) < Settings.MaxPlayersCount;
}

public class LobbySettings
{
	public required int TimePerQuestion { get; set; }
	public required bool IsOpen { get; set; }
	public required int MaxPlayersCount { get; set; }
}
public class Player
{
	public required string Id { get; init; }
	public required string Name { get; init; }
	public required bool IsOwner { get; init; }
	public required bool Invited { get; set; }
	public required DateTime ConnectedAt { get; init; }
	
}

public class LobbyQuizInfo
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
}
public enum LobbyState
{
	Created,
	Started,
	Closed
}