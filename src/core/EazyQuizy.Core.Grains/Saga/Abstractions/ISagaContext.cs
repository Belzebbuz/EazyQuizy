using Google.Protobuf;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaContext
{
	public Guid SagaCompositionId { get; }
	List<IMessage> SagaMessages { get; }
}