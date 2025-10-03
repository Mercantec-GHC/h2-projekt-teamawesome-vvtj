using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.Helpers;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

// This class provides custom authentication logic for Blazor WebAssembly.
// It manages authentication state using JWT tokens stored in browser storage.
public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;
	private readonly ISessionStorageService _sessionStorage;
	private readonly JwtSecurityTokenHandler _tokenHandler = new();
	private readonly ITokenService _tokenService;
	private const string TokenKey = "authToken";

	public CustomAuthStateProvider(ILocalStorageService localStorage, ISessionStorageService sessionStorage, ITokenService tokenService)
	{
		_localStorage = localStorage;
		_sessionStorage = sessionStorage;
		_tokenService = tokenService;
	}

	// This method is called by Blazor to get the current authentication state.
	// It checks for a JWT token in session storage first, then local storage.
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Try to get the token from session storage; if not found, check local storage.
		var savedToken = await _tokenService.GetTokenAsync();

		//If no token, return anonymous (unauthenticated) user
		if (string.IsNullOrWhiteSpace(savedToken))
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		// Check if the token is expired
		if (JWTHelper.IsTokenExpired(savedToken))
		{
			// Try to refresh the token
			var newToken = await _tokenService.RefreshAccessTokenAsync();
			if (!string.IsNullOrWhiteSpace(newToken) && !JWTHelper.IsTokenExpired(newToken))
			{
				savedToken = newToken;
			}
			else
			{
				await _localStorage.RemoveItemAsync(TokenKey);
				await _sessionStorage.RemoveItemAsync(TokenKey);
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}
		}

		//Notify the application about the authentication state change
		return GetAuthenticatedState(savedToken);
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
		NotifyAuthenticationStateChanged(Task.FromResult(GetAuthenticatedState(token)));
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
	
}
