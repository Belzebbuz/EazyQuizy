using Google.Protobuf;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaCollection
{
	public ISagaCollection AddSaga<TSaga, TRequest>()
		where TSaga : ISaga<TRequest>
		where TRequest : IMessage;
}