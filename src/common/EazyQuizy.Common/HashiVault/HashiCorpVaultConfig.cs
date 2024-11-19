namespace EazyQuizy.Common.HashiVault;

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