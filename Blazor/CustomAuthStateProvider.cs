using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;
	private readonly ISessionStorageService _sessionStorage;
	private readonly JwtSecurityTokenHandler _tokenHandler = new();
	private readonly APIService _apiService;
	private const string _tokenKey = "authToken";

	public CustomAuthStateProvider(ILocalStorageService localStorage, ISessionStorageService sessionStorage,  APIService apiService)
	{
		_localStorage = localStorage;
		_sessionStorage = sessionStorage;
		_apiService = apiService;
	}

	// Returns the current authentication state
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Try to get the token from session storage first, then local storage
		var savedToken = await _sessionStorage.GetItemAsStringAsync(_tokenKey)
			?? await _localStorage.GetItemAsStringAsync(_tokenKey);

		//If no token, return anonymous user
		if (string.IsNullOrWhiteSpace(savedToken))
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		// Remove any surrounding quotes from the token
		var cleanToken = savedToken.Trim('"');

		//Set the token in the HttpClient for future requests, so that it is available after a page refresh
		_apiService.SetBearerToken(cleanToken);

		//If token exists, parse claims and create authenticated user
		var claims = ParseClaimsFromJwt(cleanToken);

		// Create a ClaimsIdentity with the parsed claims and authentication type "jwt"
		var identity = new ClaimsIdentity(claims, "jwt");
		// Create a ClaimsPrincipal (the user) from the identity
		var user = new ClaimsPrincipal(identity);

		//Notify the application about the authentication state change
		return new AuthenticationState(user);
	}

	// Notifies the app that the user has authenticated with a given token
	public void NotifyUserAuthentication (string token)
	{
		var cleanToken = token.Trim('"');
		var claims = ParseClaimsFromJwt(cleanToken);
		var identity = new ClaimsIdentity(claims, "jwt");
		var user = new ClaimsPrincipal(identity);

		// Create an AuthenticationState for the authenticated user
		var authState = Task.FromResult(new AuthenticationState(user));

		// Notify Blazor that the authentication state has changed
		NotifyAuthenticationStateChanged(authState);
	}

	public void NotifyUserLogout()
	{
		// Create an anonymous ClaimsPrincipal (no identity)
		var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
		// Create an AuthenticationState for the anonymous user
		var authState = Task.FromResult(new AuthenticationState(anonymousUser));
		// Notify Blazor that the authentication state has changed
		NotifyAuthenticationStateChanged(authState);
	}

	//Helper method to parse claims from JWT
	private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
	{
		var token = _tokenHandler.ReadJwtToken(jwt);
		return token.Claims;
	}
}
