namespace EazyQuizy.Core.Silo.Configs;

public class SiloConfig
{
	public required ClusterConfig ClusterConfig { get; init; }
	public required RedisPersistenceConfig RedisPersistenceConfig { get; init; }
	public required MongoConfig MongoConfig { get; init; }
	public required NatsConfig NatsConfig { get; init; }
}

public class ClusterConfig
{
	public required string ConnectionString { get; init; }
	public required string ClusterId { get; init; }
	public required string ServiceId { get; init; }
}

public class RedisPersistenceConfig
{
	public required string ConnectionString { get; init; }
}
public class MongoConfig
{
	public required string ConnectionString { get; init; }
}
public class NatsConfig
{
	public required string ConnectionString { get; init; }
}
