using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Game;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Concurrency;

namespace EazyQuizy.Core.Grains.Games;

public interface IGameTimerGrain : IGrainWithGuidKey
{
	public Task StartAsync(int seconds, string eventChannel);
	public Task StopAsync();
	[ReadOnly]
	public Task<int?> GetCurrentAsync();
}

public class GameTimerGrain(NatsConnection connection, ILogger<GameTimerGrain> logger) : Grain, IGameTimerGrain
{
	private IGrainTimer? _timer;
	private string? _updateChannel;
	private int? _currentValue;
	private int? _startValue;
	public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
	{
		logger.LogError($"IAM DIYING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! {_currentValue}");
		return base.OnDeactivateAsync(reason, cancellationToken);
	}

	public Task StartAsync(int seconds, string eventChannel)
	{
		_updateChannel = eventChannel;
		_currentValue = seconds;
		_startValue = seconds;
		_timer = this.RegisterGrainTimer(TimerTick, new GrainTimerCreationOptions
		{
			DueTime = TimeSpan.Zero,
			Period = TimeSpan.FromSeconds(1),
		});
		return Task.CompletedTask;
	}

	public Task StopAsync()
	{
		_timer?.Dispose();
		_currentValue = null;
		return Task.CompletedTask;
	}

	public Task<int?> GetCurrentAsync() => Task.FromResult(_currentValue);

	private async Task TimerTick()
	{
		if(_updateChannel is null || !_currentValue.HasValue || !_startValue.HasValue)
			return;
		
		_currentValue--;
		await connection.PublishAsync(_updateChannel, new TimerUpdateEvent()
		{
			StartValue = _startValue.Value,
			Value = _currentValue.Value
		});
		if (_currentValue == 0)
		{
			var isQuestionsDone = await GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKey()).NextQuestionAsync();
			if(isQuestionsDone)
				_timer?.Dispose();
			else
			{
				_currentValue = _startValue;
			}
		}
	}
}