using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

internal class SagaProvider(IServiceProvider provider) : ISagaProvider
{
	public IReadOnlyDictionary<int, ISaga<T>> GetSagas<T>() where T : IMessage
	{
		var sagaInfo = provider.GetRequiredService<ISagaInformation>();
		var sagaOrders = sagaInfo.SagaOrder[typeof(T)];
		return provider.GetServices<ISaga<T>>()
			.OrderBy(x =>sagaOrders[x.GetType()])
			.ToDictionary(x => sagaOrders[x.GetType()])
			.AsReadOnly();
	}
}