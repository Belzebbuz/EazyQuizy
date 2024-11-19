using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Throw;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.JWT;

namespace EazyQuizy.Common.HashiVault;

public class HashiCorpVaultClient : IHashiCorpVaultClient
{
	private readonly IOptions<HashiCorpVaultConfig> _options;
	private Lazy<Task<string>> _token;

	public HashiCorpVaultClient(IOptions<HashiCorpVaultConfig> options)
	{
		_options = options;
		_token = new Lazy<Task<string>>(GetToken);
	}

	public async Task<IDictionary<string, object>> GetSecretValuesAsync(string secretPath)
	{
		var token = await _token.Value;
		IAuthMethodInfo authMethod = new JWTAuthMethodInfo("default",token);
		var vaultClientSettings = new VaultClientSettings(_options.Value.Host, authMethod)
		{
			ContinueAsyncTasksOnCapturedContext = false
		};
		var vaultClient = new VaultClient(vaultClientSettings);
		var kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2
			.ReadSecretAsync(path: $"{_options.Value.RootPath}/{secretPath}", mountPoint: _options.Value.MountPoint);
		return kv2Secret.Data.Data;
	}

	private readonly record struct KeycloakResponse(string access_token, string refresh_token, int expires_in);
	private async Task<string> GetToken()
	{
		using var client = new HttpClient();
		var values = new Dictionary<string, string>
		{
			["client_id"] = _options.Value.ClientId,
			["client_secret"] = _options.Value.ClientSecret,
			["username"] = _options.Value.Username,
			["password"] = _options.Value.Password,
			["grant_type"] = "password"
		};
		var content = new FormUrlEncodedContent(values);
		
		var response = await client.PostAsync(_options.Value.TokenUrl, content);
		var result = await response.Content.ReadFromJsonAsync<KeycloakResponse>();
		await Task.Factory.StartNew(() => RefreshAsync(result));
		return result.access_token;
	}

	private async Task RefreshAsync(KeycloakResponse tokenResponse)
	{
		await Task.Yield();
		while (true)
		{
			var delay = TimeSpan.FromSeconds(tokenResponse.expires_in) - TimeSpan.FromSeconds(30);
			await Task.Delay(delay);
			using var client = new HttpClient();
			var values = new Dictionary<string, string>
			{
				["client_id"] = _options.Value.ClientId,
				["client_secret"] = _options.Value.ClientSecret,
				["refresh_token"] = tokenResponse.refresh_token,
				["grant_type"] = "refresh_token"
			};
			var content = new FormUrlEncodedContent(values);
			var response = await client.PostAsync(_options.Value.TokenUrl, content);
			var result = await response.Content.ReadFromJsonAsync<KeycloakResponse>();
			_token = new Lazy<Task<string>>(() => Task.FromResult(result.access_token));
		}
	}
}

public class HashiCorpVaultClientCacheDecorator(IOptions<HashiCorpVaultConfig> options) : IHashiCorpVaultClient
{
	public Task<IDictionary<string, object>> GetSecretValuesAsync(string secretPath)
	{
		throw new NotImplementedException();
	}
}