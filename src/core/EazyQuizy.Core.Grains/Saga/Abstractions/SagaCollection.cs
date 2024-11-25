using System.Collections.ObjectModel;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

internal class SagaCollection(IServiceCollection services) : ISagaCollection , ISagaRegistrator
{
	private readonly Dictionary<Type, List<Type>> _messageSagaMap = [];
	private readonly SagaInformation _sagaInformation = new ();
	public ISagaCollection AddSaga<TSaga, TRequest>() where TSaga : ISaga<TRequest> where TRequest : IMessage
	{
		if (!_messageSagaMap.TryGetValue(typeof(TRequest), out var sagas))
		{
			sagas = [];
			_messageSagaMap[typeof(TRequest)] = sagas;
		}
		sagas.Add(typeof(TSaga));
		_sagaInformation.AddNext<TSaga,TRequest>();
		return this;
	}

	public void RegisterSagas()
	{
		foreach (var (type, map) in _messageSagaMap)
		{
			foreach (var sagaImplementation in map)
			{
				var sagaType = typeof(ISaga<>);
				var genericSaga = sagaType.MakeGenericType(type);
				services.AddTransient(genericSaga, sagaImplementation);
			}
		}

		services.AddSingleton<ISagaInformation>(_ => _sagaInformation);
	}
}

internal interface ISagaInformation
{
	internal IReadOnlyDictionary<Type, IReadOnlyDictionary<Type, int>> SagaOrder { get; }
}

internal class SagaInformation: ISagaInformation
{
	public IReadOnlyDictionary<Type, IReadOnlyDictionary<Type, int>> SagaOrder => _sagaOrders
		.ToDictionary(x => x.Key, 
			IReadOnlyDictionary<Type, int> (x) => new ReadOnlyDictionary<Type, int>(x.Value))
		.AsReadOnly();
	private readonly Dictionary<Type, Dictionary<Type, int>> _sagaOrders = [];
	public void AddNext<TSaga,TRequest>() where TRequest : IMessage where TSaga : ISaga<TRequest>
	{
		if (!_sagaOrders.TryGetValue(typeof(TRequest), out var sagaOrder))
		{
			sagaOrder = [];
			_sagaOrders[typeof(TRequest)] = sagaOrder;
		}
		sagaOrder.Add(typeof(TSaga), sagaOrder.Count + 1);
	}
}