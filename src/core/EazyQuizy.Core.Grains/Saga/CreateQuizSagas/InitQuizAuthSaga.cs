using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.CreateQuizSagas;

public class InitQuizAuthSaga(IGrainFactory factory) : ISaga<CreateQuizRequest>
{
	public async Task HandleAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		await grain.InitPolicyAsync();
	}

	public async Task RollbackAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<IAuthorizationGrain>(context.SagaCompositionId);
		await grain.ClearPolicyAsync();
	}
}