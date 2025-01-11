namespace EazyQuizy.Api.Configs;

public class GptServiceSettings
{
	public const string HttpClientName = "GptService";
	public const string HttpAuthClientName = "AuthGptService";
	public static string? AccessToken { get; set; }
	public required string Url { get; set; }
	public required string AuthUrl { get; set; }
	public required string ClientSecret { get; set; }
	public required string Scope { get; set; }
}