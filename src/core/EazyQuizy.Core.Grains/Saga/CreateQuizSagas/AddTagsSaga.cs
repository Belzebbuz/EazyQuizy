using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Tags;
using EazyQuizy.Core.Grains.Saga.Abstractions;

namespace EazyQuizy.Core.Grains.Saga.CreateQuizSagas;

public class AddTagsSaga(IGrainFactory factory) : ISaga<CreateQuizRequest>
{
	public async Task HandleAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ITagRepositoryGrain>(context.SagaCompositionId);
		await grain.AddTagsAsync(new AddTagsToQuizRequest()
		{
			Tags = { message.Tags },
			QuizId = context.SagaCompositionId.ToString()
		});
	}

	public async Task RollbackAsync(CreateQuizRequest message, ISagaContext context)
	{
		var grain = factory.GetGrain<ITagRepositoryGrain>(context.SagaCompositionId);
		await grain.RemoveQuizFromTagsAsync();
	}
}