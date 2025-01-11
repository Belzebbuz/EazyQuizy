using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using EazyQuizy.Api.Configs;
using ErrorOr;
using Microsoft.Extensions.Options;
using Error = ErrorOr.Error;

namespace EazyQuizy.Api.Infrastructure.Gpt;

public class GptServiceAuthHandler : DelegatingHandler
{
    private readonly GptServiceSettings _gptSettings;
    private readonly HttpClient _authClient;

    public GptServiceAuthHandler(IOptions<GptServiceSettings> options, IHttpClientFactory clientFactory)
    {
        _gptSettings = options.Value;
        _authClient = clientFactory.CreateClient(GptServiceSettings.HttpAuthClientName);

    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (GptServiceSettings.AccessToken is null)
        {
            var tokenResult = await GetTokenAsync();
            if (tokenResult.IsError)
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            GptServiceSettings.AccessToken = tokenResult.Value;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GptServiceSettings.AccessToken);
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var tokenResult = await GetTokenAsync();
            if (tokenResult.IsError)
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            GptServiceSettings.AccessToken = tokenResult.Value;
            return await SendAsync(request, cancellationToken);
        }
        return response;
    }
    private async Task<ErrorOr<string>> GetTokenAsync()
    {
        var content = new List<KeyValuePair<string, string>>() { new("scope", _gptSettings.Scope) };
        var request = new FormUrlEncodedContent(content);
        _authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _gptSettings.ClientSecret);
        _authClient.DefaultRequestHeaders.Add("RqUID", [Guid.NewGuid().ToString()]);
        var response = await _authClient.PostAsync("/api/v2/oauth",request);
        if (!response.IsSuccessStatusCode)
            return Error.Failure(await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        if(result?.AccessToken is null)
            return Error.Failure(await response.Content.ReadAsStringAsync());
        return result.AccessToken;
    }
    
    private class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}