using EazyQuizy.Common.Grpc.Types;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;

namespace EazyQuizy.Core.Grains.Common;

public abstract class StateGrain<T>(
	IStorage<T> state, 
	INatsConnection connection, 
	ILogger<StateGrain<T>> logger) : Grain, IGrainWithGuidKey, IIncomingGrainCallFilter
{
	protected IStorage<T> State { get; } = state;
	protected  Queue<object> Events { get; } = [];
	public async Task Invoke(IIncomingGrainCallContext context)
	{
		try
		{
			await context.Invoke();
			await State.WriteStateAsync();
			while (Events.TryDequeue(out var message))
			{
				await connection.PublishAsync($"{typeof(T).Name.ToLower()}.update", message);
			}
		}
		catch (Exception e)
		{
			logger.LogError(e, $"Grain state error id: {this.GetPrimaryKey().ToString()}");
			await State.ReadStateAsync();
			throw;
		}
	}

	protected void StateChanged() => Events.Enqueue(new GrainStateChangedEvent()
	{
		Id = this.GetPrimaryKey().ToString()
	});

}