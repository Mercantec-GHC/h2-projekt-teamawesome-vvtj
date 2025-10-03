using System.Net.Http.Json;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DomainModels.Dto.AuthDto;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Blazor.Services;

public class TokenService : ITokenService
{
	private readonly ILocalStorageService _localStorage;
	private readonly ISessionStorageService _sessionStorage;
	private readonly HttpClient _httpClient;
	private const string TokenKey = "authToken";

	public TokenService(ILocalStorageService localStorage, ISessionStorageService sessionStorage, HttpClient httpClient)
	{
		_localStorage = localStorage;
		_sessionStorage = sessionStorage;
		_httpClient = httpClient;
	}

	public async Task SaveTokenAsync(string token, bool rememberMe)
	{
		if (rememberMe)
		{
			await _localStorage.SetItemAsStringAsync(TokenKey, token);
			await _sessionStorage.RemoveItemAsync(TokenKey);
		}
		else
		{
			await _sessionStorage.SetItemAsStringAsync(TokenKey, token);
			await _localStorage.RemoveItemAsync(TokenKey);
		}
	}

	public async Task<string?> GetTokenAsync()
	{
		return await _sessionStorage.GetItemAsync<string>(TokenKey)
			?? await _localStorage.GetItemAsync<string>(TokenKey);
	}

	public async Task<string?> RefreshAccessTokenAsync()
	{
		var newToken = await TryRefreshAccessTokenAsync();

		if (!string.IsNullOrWhiteSpace(newToken))
		{
			var remember = await _localStorage.ContainKeyAsync(TokenKey);
			await SaveTokenAsync(newToken, remember);
		}
		return newToken;
	}

	public async Task<string?> TryRefreshAccessTokenAsync()
	{
		var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/refresh-token");
		request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

		var response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
			return null;

		var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
		return result?.AccessToken;
	}
}
