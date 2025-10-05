using System.Net.Http.Json;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DomainModels.Dto.AuthDto;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Blazor.Services;

/// <summary>
/// Provides logic for storing, retrieving, and refreshing JWT access tokens in a Blazor WebAssembly application.
/// Supports both local and session storage, and handles secure token refresh using backend APIs.
/// </summary>
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

	/// <summary>
	/// Saves the JWT access token to either local storage or session storage, depending on the user's preference.
	/// Removes the token from the other storage to ensure only one copy exists.
	/// </summary>
	/// <param name="token">The JWT access token to store.</param>
	/// <param name="rememberMe">If <c>true</c>, stores the token in local storage for persistence; otherwise, uses session storage.</param>
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

	/// <summary>
	/// Retrieves the JWT access token from session storage or local storage.
	/// Checks session storage first, then falls back to local storage if not found.
	/// </summary>
	/// <returns>
	/// The JWT access token if found; otherwise, <c>null</c>.
	/// </returns>
	public async Task<string?> GetTokenAsync()
	{
		return await _sessionStorage.GetItemAsync<string>(TokenKey)
			?? await _localStorage.GetItemAsync<string>(TokenKey);
	}

	/// <summary>
	/// Attempts to refresh the JWT access token by calling the backend API.
	/// If successful, saves the new token in the appropriate storage.
	/// </summary>
	/// <returns>
	/// The new JWT access token if the refresh was successful; otherwise, <c>null</c>.
	/// </returns>
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

	/// <summary>
	/// Sends a refresh token request to the backend API and retrieves a new JWT access token if available.
	/// </summary>
	/// <returns>
	/// The new JWT access token if the refresh was successful; otherwise, <c>null</c>.
	/// </returns>
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
