using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public class AuthMessageHandler : DelegatingHandler
{
	private readonly AuthenticationStateProvider _authStateProvider;
	private readonly ILogger<AuthMessageHandler> _logger;
	private readonly ITokenService _tokenService;
	private readonly JwtSecurityTokenHandler _tokenHandler = new();
	public AuthMessageHandler(ILocalStorageService localStorage, ISessionStorageService sessionStorage, AuthenticationStateProvider authStateProvider, ITokenService tokenService, ILogger<AuthMessageHandler> logger)
	{
		_authStateProvider = authStateProvider;
		_tokenService = tokenService;
		_logger = logger;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Get the token from the storage
		var token = await _tokenService.GetTokenAsync();

		// If we have a token, attach it to the request headers
		if (!string.IsNullOrWhiteSpace(token))
		{
			// Check if token will expire in next 60 seconds → refresh proactively
			if (WillExpireSoon(token))
			{
				_logger.LogInformation("Access token is about to expire. Attempting proactive refresh...");
				var refreshedToken = await _tokenService.RefreshAccessTokenAsync();

				if (!string.IsNullOrWhiteSpace(refreshedToken))
					token = refreshedToken;
			}
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		else
		{
			_logger.LogWarning("No auth token found for the request to {Url}", request.RequestUri);
		}

		var response = await base.SendAsync(request, cancellationToken);

		//If unathorized, try a silent refresh token and retry once
		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			var newToken = await _tokenService.RefreshAccessTokenAsync();

			if (!string.IsNullOrWhiteSpace(newToken))
			{
				response.Dispose();

				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
				response = await base.SendAsync(request, cancellationToken);
			}
			else
			{
				_logger.LogError("Token refresh failed. User will be logged out.");
				var customStateProvider = _authStateProvider as CustomAuthStateProvider;
				customStateProvider?.NotifyUserLogout();
			}
		}

		return response;
	}

	private bool WillExpireSoon(string token)
	{
		try
		{
			var jwt = _tokenHandler.ReadJwtToken(token);
			var expiry = jwt.ValidTo; // UTC
			return expiry < DateTime.UtcNow.AddSeconds(60);
		}
		catch
		{
			return true; // invalid token -> treat as expiring
		}
	}

}
