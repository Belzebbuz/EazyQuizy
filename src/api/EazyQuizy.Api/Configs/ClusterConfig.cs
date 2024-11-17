namespace EazyQuizy.Api.Configs;

public class ClusterConfig
{
	public required string ConnectionString { get; init; }
	public required string ClusterId { get; init; }
	public required string ServiceId { get; init; }
}

public class HashiCorpVaultConfig
{
	public required string Host { get; init; }
	public required string MountPoint { get; init; }
	public required string RootPath { get; init; }
	public required string ClientId { get; init; }
	public required string ClientSecret { get; init; }
	public required string Username { get; init; }
	public required string Password { get; init; }
	public required string TokenUrl { get; init; }
}

public class MinioOptions
{
	public required string Host { get; init; }
	public required string User { get; init; }
	public required string Password { get; init; }
}