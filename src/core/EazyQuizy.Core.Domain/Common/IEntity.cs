namespace EazyQuizy.Core.Domain.Common;

public interface IEntity;

public interface IEntity<out T> : IEntity where T: notnull
{
	public T Id { get; }
}

