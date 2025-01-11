using EazyQuizy.Common.Grpc.Lobby;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Grains.Saga.Abstractions;
using EazyQuizy.Core.Grains.Saga.CreateLobbySagas;
using EazyQuizy.Core.Grains.Saga.CreateQuizSagas;
using EazyQuizy.Core.Grains.Saga.StartGameSagas;

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

		collection
			.AddSaga<InitLobbyAuthSaga, CreateLobbyRequest>()
			.AddSaga<CreateLobbySaga, CreateLobbyRequest>();

		collection
			.AddSaga<InitGameAuthSaga, CreateGameRequest>()
			.AddSaga<CreateGameSaga,CreateGameRequest>()
			.AddSaga<SetGameIdSaga, CreateGameRequest>();
	}
}