namespace EazyQuizy.Core.Domain.Common;

public class DomainEvent : IEvent
{
	public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}