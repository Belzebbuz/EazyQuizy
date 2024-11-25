using Google.Protobuf;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaProvider
{
	public IReadOnlyDictionary<int,ISaga<T>> GetSagas<T>() where T : IMessage;
}