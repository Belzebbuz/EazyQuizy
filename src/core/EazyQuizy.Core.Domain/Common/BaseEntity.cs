using System.ComponentModel.DataAnnotations.Schema;

namespace EazyQuizy.Core.Domain.Common;

public abstract class BaseEntity<T> : IEntity<T> where T: notnull
{
	public required T Id { get; set; }
	[NotMapped]
	private readonly Queue<DomainEvent> _domainEvents = new();

	public void Enqueue(DomainEvent @event) => _domainEvents.Enqueue(@event);

	public IEnumerable<DomainEvent> ReadEvents()
	{
		while (_domainEvents.TryDequeue(out var @event))
		{
			yield return @event;
		}
	}

}