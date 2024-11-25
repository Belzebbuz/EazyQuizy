using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Grains.Saga.Abstractions;
using NATS.Client.Core;
using Throw;

namespace EazyQuizy.Core.Grains.Saga.CreateQuizSagas;

public class PublicMessageSaga(INatsConnection connection) : ISaga<CreateQuizRequest>
{
	public async Task HandleAsync(CreateQuizRequest message, ISagaContext context)
	{
		var userId = RequestContext.Get("userId") as string;
		userId.ThrowIfNull();
		await connection.PublishAsync($"quiz.update.{userId}", new GrainStateChangedEvent()
		{
			Id = context.SagaCompositionId.ToString()
		});
	}

	public Task RollbackAsync(CreateQuizRequest message, ISagaContext context)
	{
		return Task.CompletedTask;
	}
}