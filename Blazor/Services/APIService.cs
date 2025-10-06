using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Blazor.Services;

/// <summary>
/// Provides HTTP client helper methods for communicating with the backend API in a Blazor WebAssembly application.
/// Supports sending requests with or without credentials and handles common HTTP operations.
/// </summary>
public partial class APIService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<APIService> _logger;

	public APIService(HttpClient httpClient, ILogger<APIService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	/// <summary>
	/// Sends a POST request with a JSON body and includes browser credentials (cookies).
	/// Useful for login operations where the backend sets authentication cookies.
	/// </summary>
	/// <typeparam name="T">The type of the request body.</typeparam>
	/// <param name="url">The endpoint URL to which the request is sent.</param>
	/// <param name="body">The request body to serialize as JSON.</param>
	/// <returns>
	/// A <see cref="HttpResponseMessage"/> representing the HTTP response from the backend.
	/// </returns>
	public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T body)
	{
		try
		{
			var request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = JsonContent.Create(body)
			};
			request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

			return await _httpClient.SendAsync(request);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "POST request failed: {Url}", url);
			throw;
		}
	}

	/// <summary>
	/// Sends a POST request without a body but includes browser credentials (cookies).
	/// Useful for operations like refresh-token where the backend reads the refresh token from an HttpOnly cookie.
	/// </summary>
	/// <param name="url">The endpoint URL to which the request is sent.</param>
	/// <returns>
	/// A <see cref="HttpResponseMessage"/> representing the HTTP response from the backend.
	/// </returns>
	public async Task<HttpResponseMessage> PostWithCredentialsAsync(string url)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url);
		request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

		return await _httpClient.SendAsync(request);
	}

	/// <summary>
	/// Sends a PUT request with a JSON body to the specified URL.
	/// </summary>
	/// <typeparam name="T">The type of the request body.</typeparam>
	/// <param name="url">The endpoint URL to which the request is sent.</param>
	/// <param name="body">The request body to serialize as JSON.</param>
	/// <returns>
	/// A <see cref="HttpResponseMessage"/> representing the HTTP response from the backend.
	/// </returns>
	public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string url, T body)
	{
		try
		{
			return await _httpClient.PutAsJsonAsync(url, body);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "PUT request failed: {Url}", url);
			throw;
		}
	}

	/// <summary>
	/// Sends a GET request to the specified URL and returns the HTTP response.
	/// </summary>
	/// <param name="url">The endpoint URL to which the request is sent.</param>
	/// <returns>
	/// A <see cref="HttpResponseMessage"/> representing the HTTP response from the backend.
	/// </returns>
	public async Task<HttpResponseMessage> GetAsync(string url)
	{
		try
		{
			return await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "GET request failed: {Url}", url);
			throw;
		}
	}

}
