using System.Net.Http.Headers;
using System.Text.Json;
using EazyQuizy.Api.Configs;
using EazyQuizy.Api.Infrastructure.Gpt.Messages;
using ErrorOr;
namespace EazyQuizy.Api.Infrastructure.Gpt;

public interface IGptService
{
	Task<ErrorOr<string>> SendMessageAsync(string prompt, string? systemText);
}
public class GptService : IGptService
{
	private readonly HttpClient _client;
	private readonly ILogger<GptService> _logger;

	public GptService(
		IHttpClientFactory clientFactory, 
		ILogger<GptService> logger)
	{
		_client = clientFactory.CreateClient(GptServiceSettings.HttpClientName);
		_logger = logger;
	}
	public async Task<ErrorOr<string>> SendMessageAsync(string prompt, string? systemText)
	{
		try
		{
			var request = new GptRequest(systemText,prompt);
			_client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", GptServiceSettings.AccessToken);
			var response = await _client.PostAsJsonAsync("/api/v1/chat/completions", request);
			var jsonBody = await response.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<GptResponse>(jsonBody);
			if (result == null || result.Choices == null)
				return Error.Failure(description: "Не удалось получить ответ от нейронной сети");
			if (!result.Choices.Any())
				return Error.Failure(description: "Не удалось получить ответ от нейронной сети");
			var messageContent = result.Choices[0].Message?.Content;
			if (string.IsNullOrEmpty(messageContent))
				return Error.Failure(description: "Не удалось получить ответ от нейронной сети");
			return messageContent;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Error.Failure(description: "Не удалось получить ответ от нейронной сети");
		}
	}
}