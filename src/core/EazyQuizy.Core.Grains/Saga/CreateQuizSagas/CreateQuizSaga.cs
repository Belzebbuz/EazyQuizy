using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.CreateQuizSagas;

internal sealed class CreateQuizSaga(IGrainFactory factory) : ISaga<CreateQuizRequest>
{
	public async Task HandleAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IQuizRepositoryGrain>(context.SagaCompositionId);
		await grain.CreateAsync(message);
	}

	public async Task RollbackAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IQuizRepositoryGrain>(context.SagaCompositionId);
		await grain.DeleteAsync();
	}
}