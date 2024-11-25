namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public interface ISagaConfiguration
{
	public void Configure(ISagaCollection collection);
}