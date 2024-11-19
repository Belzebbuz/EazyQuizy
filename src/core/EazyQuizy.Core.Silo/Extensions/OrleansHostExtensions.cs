using EazyQuizy.Core.Grains.Constants;
using EazyQuizy.Core.Silo.Configs;
using Orleans.Configuration;
using Orleans.Serialization;
using OrleansDashboard;
using Throw;

namespace EazyQuizy.Core.Silo.Extensions;

public static class OrleansHostExtensions
{
	internal static IHostBuilder AddOrleans(this IHostBuilder builder)
	{
		builder.UseOrleans((hostBuilder, silo) =>
		{
			var siloSettings = hostBuilder.Configuration.GetSection(nameof(SiloConfig)).Get<SiloConfig>();
			siloSettings.ThrowIfNull("Не установлены настройки Silo");
			silo.Services.AddSerializer(sb => sb.AddProtobufSerializer(
				type => type.Namespace != null && type.Namespace.StartsWith("EazyQuizy.Common.Grpc"),
				type =>  type.Namespace != null && type.Namespace.StartsWith("EazyQuizy.Common.Grpc")));
			silo.UseRedisClustering(options => options.ConfigurationOptions = new()
				{
					EndPoints = [new(siloSettings.ClusterConfig.ConnectionString)]
				})
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = siloSettings.ClusterConfig.ClusterId;
					options.ServiceId = siloSettings.ClusterConfig.ServiceId;
				})
				.AddRedisGrainStorage(StorageConstants.RedisStorage, options =>
				{
					options.ConfigurationOptions = new()
					{
						EndPoints = [new(siloSettings.RedisPersistenceConfig.ConnectionString)]
					};
				})
				.ConfigureLogging(logging => logging.AddConsole());
			
				var dashboardOptions = hostBuilder.Configuration.GetSection(nameof(DashboardOptions)).Get<DashboardOptions>();
				if(dashboardOptions is not null)
					silo.UseDashboard(options =>
					{
						options.Username = dashboardOptions.Username;
						options.Password = dashboardOptions.Password;
						options.Host = dashboardOptions.Host;
						options.Port = dashboardOptions.Port;
						options.HostSelf = dashboardOptions.HostSelf;
						options.CounterUpdateIntervalMs = dashboardOptions.CounterUpdateIntervalMs;
					});
		});
		return builder;
	}
}