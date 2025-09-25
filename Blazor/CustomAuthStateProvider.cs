using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.Helpers;
using Blazor.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DomainModels.Dto.AuthDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

// This class provides custom authentication logic for Blazor WebAssembly.
// It manages authentication state using JWT tokens stored in browser storage.
public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;
	private readonly ISessionStorageService _sessionStorage;
	private readonly JwtSecurityTokenHandler _tokenHandler = new();
	private readonly APIService _apiService;
	private const string _tokenKey = "authToken";

	public CustomAuthStateProvider(ILocalStorageService localStorage, ISessionStorageService sessionStorage, APIService apiService)
	{
		_localStorage = localStorage;
		_sessionStorage = sessionStorage;
		_apiService = apiService;
	}

	// This method is called by Blazor to get the current authentication state.
	// It checks for a JWT token in session storage first, then local storage.
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Try to get the token from session storage; if not found, check local storage.
		var savedToken = await GetTokenAsync();

		//If no token, return anonymous (unauthenticated) user
		if (string.IsNullOrWhiteSpace(savedToken))
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		// Remove any surrounding quotes from the token
		var cleanToken = savedToken.Trim('"');

		// Check if the token is expired
		if (JWTHelper.IsTokenExpired(cleanToken))
		{
			// Try to refresh the token
			var newToken = await RefreshAccessTokenAsync();
			if (!string.IsNullOrWhiteSpace(newToken) && !JWTHelper.IsTokenExpired(newToken))
			{
				cleanToken = newToken;
			}
			else
			{
				await _localStorage.RemoveItemAsync(_tokenKey);
				await _sessionStorage.RemoveItemAsync(_tokenKey);
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}
		}
		//Set the token in the HttpClient for future requests, so that it is available after a page refresh
		_apiService.SetBearerToken(cleanToken);

		//Notify the application about the authentication state change
		return GetAuthenticatedState(cleanToken);
	}

	public async Task<string?> RefreshAccessTokenAsync()
	{
		var newToken = await TryRefreshAccessTokenAsync();

		if (!string.IsNullOrWhiteSpace(newToken))
		{
			var remember = await _localStorage.ContainKeyAsync(_tokenKey);
			await SaveTokenAsync(newToken, remember);

			_apiService.SetBearerToken(newToken);
			NotifyUserAuthentication(newToken);
		}
		return newToken;
	}

	public async Task<string?> TryRefreshAccessTokenAsync()
	{
		// The backend should read the refresh token from the cookie
		var response = await _apiService.PostAsJsonAsync<object>("api/Auth/refresh-token", null);

		if (!response.IsSuccessStatusCode)
			return null;

		var json = await response.Content.ReadAsStringAsync();
		var result = System.Text.Json.JsonSerializer.Deserialize<TokenResponseDto>(json);

		return result?.AccessToken;
	}


	// This helper method creates an AuthenticationState from a JWT token.
	// It parses the token, extracts claims, and builds a ClaimsPrincipal.
	private AuthenticationState GetAuthenticatedState(string jwt)
	{
		try
		{
			// Parse the JWT token to extract claims.
			var token = _tokenHandler.ReadJwtToken(jwt);

			// Map the "role" claim to ClaimTypes.Role for Blazor's role-based authorization.
			var claims = token.Claims.Select(c => c.Type == "role"
			? new Claim(ClaimTypes.Role, c.Value) : c);

			// Create a ClaimsIdentity with the extracted claims and authentication type "jwt".
			var identity = new ClaimsIdentity(claims, "jwt");

			// Return the authentication state for the authenticated user.
			return new AuthenticationState(new ClaimsPrincipal(identity));
		}
		catch (Exception)
		{
			// If token is invalid or cannot be parsed, return an anonymous (unauthenticated) user.
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}
	}

	// Call this method when a user successfully logs in.
	// It notifies Blazor that the authentication state has changed.
	public void NotifyUserAuthentication(string token)
	{
		var cleanToken = token.Trim('"');
		NotifyAuthenticationStateChanged(Task.FromResult(GetAuthenticatedState(cleanToken)));
	}

	// Call this method when a user logs out.
	// It notifies Blazor that the user is now anonymous.
	public void NotifyUserLogout()
	{
		// Create an anonymous ClaimsPrincipal (no identity)
		var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
		// Create an AuthenticationState for the anonymous user
		var authState = Task.FromResult(new AuthenticationState(anonymousUser));
		// Notify Blazor that the authentication state has changed
		NotifyAuthenticationStateChanged(authState);
	}
	public async Task SaveTokenAsync(string token, bool rememberMe)
	{

		if (rememberMe)
		{
			await _localStorage.SetItemAsync("authToken", token);
			await _sessionStorage.RemoveItemAsync("authToken");
		}
		else
		{
			await _sessionStorage.SetItemAsync("authToken", token);
			await _localStorage.RemoveItemAsync("authToken");
		}
	}

	public async Task<string?> GetTokenAsync()
	{
		return await _sessionStorage.GetItemAsync<string>("authToken")
			?? await _localStorage.GetItemAsync<string>("authToken");
	}
}
