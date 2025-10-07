using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.Helpers;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

/// <summary>
/// Provides custom authentication state management for Blazor WebAssembly applications.
/// Handles authentication state using JWT tokens stored in browser storage, supports token refresh,
/// and notifies the application of authentication state changes.
/// </summary>
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

	/// <summary>
	/// Gets the current authentication state for the application.
	/// Checks for a JWT token in session storage first, then local storage.
	/// If the token is expired, attempts to refresh it. Returns an anonymous user if no valid token is found.
	/// </summary>
	/// <returns>
	/// An <see cref="AuthenticationState"/> representing the current user's authentication state.
	/// </returns>
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

	/// <summary>
	/// Creates an <see cref="AuthenticationState"/> from a JWT token.
	/// Parses the token, extracts claims, and builds a <see cref="ClaimsPrincipal"/>.
	/// </summary>
	/// <param name="jwt">The JWT access token string.</param>
	/// <returns>
	/// An <see cref="AuthenticationState"/> for the authenticated user, or an anonymous user if the token is invalid.
	/// </returns>
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

	/// <summary>
	/// Notifies the application that the user has successfully authenticated.
	/// Triggers a re-evaluation of the authentication state with the provided token.
	/// </summary>
	/// <param name="token">The JWT access token for the authenticated user.</param>
	public void NotifyUserAuthentication(string token)
	{
		NotifyAuthenticationStateChanged(Task.FromResult(GetAuthenticatedState(token)));
	}

	/// <summary>
	/// Notifies the application that the user has logged out.
	/// Triggers a re-evaluation of the authentication state as anonymous.
	/// </summary>
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
