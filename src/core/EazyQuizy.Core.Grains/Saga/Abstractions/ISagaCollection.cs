using Google.Protobuf;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaCollection
{
	/// <summary>
	/// Регистрирует сагу в том порядке в котором вызывается этот метод <br/>
	/// Вызов компенсации производится в обратном порядке 
	/// </summary>
	/// <typeparam name="TSaga">Сага</typeparam>
	/// <typeparam name="TRequest">Запрос, который обрабатывает композиция саги</typeparam>
	/// <returns></returns>
	public ISagaCollection AddSaga<TSaga, TRequest>()
		where TSaga : ISaga<TRequest>
		where TRequest : IMessage;
}