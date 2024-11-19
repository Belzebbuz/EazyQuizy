using EazyQuizy.Api.Configs;
using Orleans.Configuration;
using Orleans.Serialization;
using StackExchange.Redis;

namespace EazyQuizy.Api.Extensions;

public static class OrleansExtensions
{
	public static IHostBuilder AddOrleansClient(this IHostBuilder builder, IConfiguration config)
	{

		builder.UseOrleansClient(client =>
		{
			var orleansSettings = config.GetSection(nameof(ClusterConfig)).Get<ClusterConfig>()
			                      ?? throw new ArgumentNullException(nameof(ClusterConfig));
			client.Services.AddSerializer(sb => sb.AddProtobufSerializer(
				type => type.Namespace != null && type.Namespace.StartsWith("EazyQuizy.Common.Grpc"),
				type =>  type.Namespace != null && type.Namespace.StartsWith("EazyQuizy.Common.Grpc")));
			client
				.UseRedisClustering(options => options.ConfigurationOptions = new()
				{
					EndPoints = [new(orleansSettings.ConnectionString)]
				})
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = orleansSettings.ClusterId;
					options.ServiceId = orleansSettings.ServiceId;
				});
		});
		return builder;
	}
}