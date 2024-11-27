using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EazyQuizy.Core.Grains.Saga.Abstractions;

public static class ServiceCollectionExtensions
{
	//TODO
	//А если разные сборки? SagaCollection должен быть static?
	public static IServiceCollection AddSagas(this IServiceCollection services, Assembly assembly)
	{
		var configurations = assembly.GetTypes()
			.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(i =>
				i == typeof(ISagaConfiguration)));
		var sagaCollection = new SagaCollection(services);
		foreach (var configType in configurations)
		{
			var config = Activator.CreateInstance(configType) as ISagaConfiguration
			             ?? throw new ArgumentNullException();
			config.Configure(sagaCollection);
		}
		sagaCollection.RegisterSagas();
		services.AddSingleton<ISagaProvider, SagaProvider>();
		return services;
	}
}