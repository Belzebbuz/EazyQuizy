using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.SagaOrchestrator;
using EazyQuizy.Core.Domain.Exceptions;
using EazyQuizy.Core.Grains.Saga.Abstractions;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace EazyQuizy.Core.Grains.Saga;

public record SagaState(Guid Id, Guid SagaId, string SagaKey);
public class SagaOrchestratorGrain(
	ILogger<SagaOrchestratorGrain> logger,
	ISagaProvider provider) : Grain, ISagaOrchestratorGrain
{
	private class SagaContext(Guid sagaCompositionId) : ISagaContext
	{
		public Guid SagaCompositionId { get; } = sagaCompositionId;
		public Stack<int> EndedSagas { get; } = [];
	}
	public Task<StatusResponse> EvaluateSagaAsync(CreateQuizRequest request) 
		=> StartAsync(request);

	private async Task<StatusResponse> StartAsync<TRequest>(TRequest request)
		where TRequest : IMessage
	{
		var sagas = provider.GetSagas<TRequest>();
		var context = new SagaContext(this.GetPrimaryKey());
		try
		{
			foreach (var saga in sagas)
			{
				await saga.Value.HandleAsync(request, context);
				context.EndedSagas.Push(saga.Key);
				logger.LogInformation($"SagaComposition:{context.SagaCompositionId}. Saga {saga.Value.GetType().FullName} completed");
			}

			return new StatusResponse()
			{
				OperationId = this.GetPrimaryKey().ToString(),
				Succeeded = true
			};
		}
		catch (Exception e)
		{
			while (context.EndedSagas.TryPop(out var key))
			{
				await sagas[key].RollbackAsync(request, context);
			}
			var response = new StatusResponse()
			{
				OperationId = this.GetPrimaryKey().ToString(),
				Succeeded = false,
			};
			if (e is DomainException domainException)
				response.Message = domainException.Message;
			else
				response.Message = "Внутренняя ошибка";
			return response;
		}
	}
}