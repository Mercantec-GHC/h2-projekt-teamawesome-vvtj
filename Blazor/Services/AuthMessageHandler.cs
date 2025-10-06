using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Blazor.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

/// <summary>
/// A delegating HTTP message handler that automatically attaches and refreshes JWT access tokens for outgoing HTTP requests.
/// Handles proactive token refresh, silent refresh on 401 responses, and user logout on refresh failure.
/// Designed for use in Blazor WebAssembly applications with authentication.
/// </summary>
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

	/// <summary>
	/// Sends an HTTP request with an attached JWT access token if available.
	/// Proactively refreshes the token if it is about to expire, and attempts a silent refresh and retry on 401 Unauthorized responses.
	/// Logs the user out if token refresh fails.
	/// </summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>
	/// A <see cref="HttpResponseMessage"/> representing the HTTP response from the backend.
	/// </returns>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Get the token from the storage
		var token = await _tokenService.GetTokenAsync();

		// If we have a token, attach it to the request headers
		if (!string.IsNullOrWhiteSpace(token))
		{
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

	/// <summary>
	/// Determines whether the provided JWT access token will expire within the next 60 seconds.
	/// Used to trigger proactive token refresh before sending a request.
	/// </summary>
	/// <param name="token">The JWT access token string.</param>
	/// <returns>
	/// <c>true</c> if the token will expire soon or is invalid; otherwise, <c>false</c>.
	/// </returns>
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
