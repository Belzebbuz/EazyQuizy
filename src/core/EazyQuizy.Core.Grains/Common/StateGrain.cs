using System.Net;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Exceptions;
using JasperFx.Core.Reflection;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;

namespace EazyQuizy.Core.Grains.Common;

public class ThrowIfNoRecordAttribute : Attribute;
public class StateChangerAttribute : Attribute;
public abstract class StateGrain<T>(
	IStorage<T> state, 
	INatsConnection connection, 
	ILogger<StateGrain<T>> logger) : Grain, IGrainWithGuidKey, IIncomingGrainCallFilter
{
	protected IStorage<T> State { get; } = state;
	private readonly Queue<object> _events = [];
	public async Task Invoke(IIncomingGrainCallContext context)
	{
		ThrowIfRecordNotExists(context);
		try
		{
			await context.Invoke();
			await SaveChangesAsync(context);
			await SendEventAsync();
		}
		catch (Exception e)
		{
			logger.LogError(e, $"Grain state error id: {this.GetPrimaryKey().ToString()}");
			await State.ReadStateAsync();
			throw;
		}
	}

	private async Task SendEventAsync()
	{
		while (_events.TryDequeue(out var message))
		{
			await connection.PublishAsync(GetUpdateChannel(), message);
		}
	}

	private async Task SaveChangesAsync(IIncomingGrainCallContext context)
	{
		if (context.ImplementationMethod.GetAttribute<StateChangerAttribute>() is null) return;
		await State.WriteStateAsync();
		StateChanged();
		
	}

	private void ThrowIfRecordNotExists(IIncomingGrainCallContext context)
	{
		if (context.ImplementationMethod.GetAttribute<ThrowIfNoRecordAttribute>() is null) return;
		if(!State.RecordExists)
			throw DomainException.Create("Объект не существует", HttpStatusCode.NotFound);
	}

	protected virtual string GetUpdateChannel() => 	$"{typeof(T).Name.ToLower()}.update.{this.GetPrimaryKey()}";
	private void StateChanged() => _events.Enqueue(new GrainStateChangedEvent()
	{
		Id = this.GetPrimaryKey().ToString()
	});
}