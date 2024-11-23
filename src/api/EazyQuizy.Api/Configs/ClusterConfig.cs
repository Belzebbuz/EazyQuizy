namespace EazyQuizy.Api.Configs;

public class ClusterConfig
{
	public required string ConnectionString { get; init; }
	public required string ClusterId { get; init; }
	public required string ServiceId { get; init; }
}

public class MinioOptions
{
	public required string Host { get; init; }
	public required string User { get; init; }
	public required string Password { get; init; }
}
