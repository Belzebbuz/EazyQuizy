using Google.Protobuf;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISaga<in TRequest>
	where TRequest: IMessage 
{
	public Task HandleAsync(TRequest message, ISagaContext context);
	public Task RollbackAsync(TRequest message, ISagaContext context);
}