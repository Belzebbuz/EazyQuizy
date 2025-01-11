using System.Text.Json;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using EazyQuizy.Core.Grains.Saga;
using EazyQuizy.Core.Grains.Saga.Abstractions;
using EazyQuizy.Core.Silo.Configs;
using Marten;
using NATS.Client.Core;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Orleans.Configuration;
using Orleans.Serialization;
using Throw;
using Weasel.Postgresql.Tables;

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
			silo.Services.AddNats(1, opt => new NatsOpts()
			{
				Url = siloSettings.NatsConfig.ConnectionString,
				SerializerRegistry = new CastomNatsJsonSerializerRegistry(new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				})
			});
			silo.Services.AddMarten(options =>
			{
				options.Connection(siloSettings.PostgresConfig.ConnectionString);
				options.UseSystemTextJsonForSerialization(new JsonSerializerOptions()
				{
					IgnoreReadOnlyFields = true,
					IgnoreReadOnlyProperties = true,
					AllowOutOfOrderMetadataProperties = true
				});
				options.Schema.For<SagaState>().Index(x => x.SagaId, x => x.SortOrder = SortOrder.Desc);
				options.Schema.For<Tag>().Index(x => x.Name);
				options.Schema.For<Quiz>().Index(x => x.UserId);
				options.Schema.For<Quiz>().Index(x => x.ModifiedAt, x => x.SortOrder = SortOrder.Desc);
			});
			silo.Services.AddSagas(typeof(SagaOrchestratorGrain).Assembly);
			silo.AddIncomingGrainCallFilter<AuthGrainFilter>();
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
		});
		return builder;
	}
}

public sealed class CastomNatsJsonSerializerRegistry(JsonSerializerOptions options) : INatsSerializerRegistry
{
	public INatsSerialize<T> GetSerializer<T>() => new NatsJsonSerializer<T>(options);

	public INatsDeserialize<T> GetDeserializer<T>() => new NatsJsonSerializer<T>(options);
}
