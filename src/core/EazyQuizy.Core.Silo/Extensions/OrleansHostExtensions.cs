using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using EazyQuizy.Core.Silo.Configs;
using EazyQuizy.Core.Silo.MongoConfig;
using MongoDB.Bson.Serialization;
using NATS.Client.Core;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.StorageProviders.Serializers;
using Orleans.Serialization;
using Throw;

namespace EazyQuizy.Core.Silo.Extensions;

public static class OrleansHostExtensions
{
	internal static IHostBuilder AddOrleans(this IHostBuilder builder)
	{
		MongoDbClassMap.Initialize();
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
				SerializerRegistry = NatsJsonSerializerRegistry.Default
			});
			silo.Services.AddKeyedSingleton<IGrainStateSerializer,BsonGrainStateSerializer>(StorageConstants.MongoDbStorage);
			silo.UseMongoDBClient(siloSettings.MongoConfig.ConnectionString);
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
				.AddMongoDBGrainStorage(StorageConstants.MongoDbStorage, options =>
				{
					options.DatabaseName = StorageConstants.MongoDbStorage;
					options.CreateShardKeyForCosmos = true;
				})
				.ConfigureLogging(logging => logging.AddConsole());
		});
		return builder;
	}
}