using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;
	private readonly JwtSecurityTokenHandler _tokenHandler = new();
	private readonly HttpClient _httpClient;
	private const string _tokenKey = "authToken";

	public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
	{
		_localStorage = localStorage;
		_httpClient = httpClient;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		//Check if token exists in local storage
		var savedToken = await _localStorage.GetItemAsStringAsync(_tokenKey);

		//If no token, return anonymous user
		if (string.IsNullOrWhiteSpace(savedToken))
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		//Set the token in the HttpClient for future requests, so that it is available after a page refresh
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

		//If token exists, parse claims and create authenticated user
		var claims = ParseClaimsFromJwt(savedToken);

		var identity = new ClaimsIdentity(claims, "jwt");
		var user = new ClaimsPrincipal(identity);

		//Notify the application about the authentication state change
		return new AuthenticationState(user);
	}

	public void NotifyUserAuthentication (string token)
	{
		var claims = ParseClaimsFromJwt(token);
		var identity = new ClaimsIdentity(claims, "jwt");
		var user = new ClaimsPrincipal(identity);

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public void NotifyUserLogout()
	{
		var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
	}

	//Helper method to parse claims from JWT
	private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
	{
		var token = _tokenHandler.ReadJwtToken(jwt);
		return token.Claims;
	}
}
