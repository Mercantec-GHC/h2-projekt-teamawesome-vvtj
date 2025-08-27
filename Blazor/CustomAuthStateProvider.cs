using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;
	private readonly HttpClient _httpClient;
	private const string _tokenKey = "authToken";

	public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
	{
		_localStorage = localStorage;
		_httpClient = httpClient;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Try to get token from localStorage
		var savedToken = await _localStorage.GetItemAsStringAsync(_tokenKey);

		// If no token, return anonymous user
		if (string.IsNullOrWhiteSpace(savedToken))
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		// If token exists, validate and parse it
		try
		{
			//Parse token
			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadJwtToken(savedToken.Replace("\"", ""));

			//Set header
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken.Replace("\"", ""));

			//Create user identity
			var identity = new ClaimsIdentity(token.Claims, "jwt");
			var user = new ClaimsPrincipal(identity);

			return new AuthenticationState(user);
		}
		catch
		{
			// If token is invalid, clear it
			await _localStorage.RemoveItemAsync(_tokenKey);
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}
	}

	//Called after login
	public async Task MarkUserAsAuthenticated(string token)
	{
		// Save token to localStorage
		await _localStorage.SetItemAsStringAsync(_tokenKey, token);

		// Notify authentication state changed
		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
	}

	//Called after logout
	public async Task MarkUserAsLoggedOut()
	{
		// Remove token from localStorage
		await _localStorage.RemoveItemAsync(_tokenKey);

		// Clear the authorization header
		_httpClient.DefaultRequestHeaders.Authorization = null;

		// Notify authentication state changed
		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
	}
}
