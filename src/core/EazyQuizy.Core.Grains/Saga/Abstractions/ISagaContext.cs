namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaContext
{
	public Guid SagaCompositionId { get; }
}