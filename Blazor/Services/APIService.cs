using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Blazor.Services
{
	public partial class APIService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<APIService> _logger;

		public APIService(HttpClient httpClient, ILogger<APIService> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T body)
		{
			try
			{
				return await _httpClient.PostAsJsonAsync(url, body);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "POST request failed: {Url}", url);
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
			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);
		}

		public void RemoveBearerToken()
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
		}
	}
}
