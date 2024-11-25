using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Grains.Saga.Abstractions;
using EazyQuizy.Core.Grains.Saga.CreateQuizSagas;

namespace EazyQuizy.Core.Grains.Saga;

public class SagaConfiguration : ISagaConfiguration
{
	public void Configure(ISagaCollection collection)
	{
		collection
			.AddSaga<InitQuizAuthSaga, CreateQuizRequest>()
			.AddSaga<CreateQuizSaga, CreateQuizRequest>()
			.AddSaga<AddTagsSaga, CreateQuizRequest>()
			.AddSaga<PublicMessageSaga, CreateQuizRequest>();
	}
}