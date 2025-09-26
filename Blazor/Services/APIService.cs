using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Blazor.Services;

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
	/// POST JSON normally (without cookies).
	/// Useful for login where backend also sets the cookie.
	/// </summary>
	public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T body)
	{
		try
		{
			// Important: include cookies for login too
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
	/// POST without body but with cookies.
	/// Useful for refresh-token calls where backend reads refresh token from HttpOnly cookie.
	/// </summary>
	public async Task<HttpResponseMessage> PostWithCredentialsAsync(string url)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url);
		request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

		return await _httpClient.SendAsync(request);
	}

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

	//Helper methods to manage the Bearer token in the HttpClient by using the APIService
	public void SetBearerToken(string token)
	{
		var cleanToken = token.Trim('"');
		_httpClient.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", cleanToken);
	}

	public void RemoveBearerToken()
	{
		_httpClient.DefaultRequestHeaders.Authorization = null;
	}
}
