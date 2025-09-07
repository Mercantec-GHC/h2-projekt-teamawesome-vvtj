using System.Net;
using System.Net.Http.Json;
using DomainModels.Dto.UserDto;

namespace Blazor.Services;

public partial class APIService
{
	public async Task<UserDto?> GetCurrentUserAsync()
	{
		Console.WriteLine("Current Authorization header: " +
	_httpClient.DefaultRequestHeaders.Authorization?.ToString());
		try
		{
			// Ensure HttpClient is properly configured for credentials/cookies if needed
			var response = await GetAsync("api/Users/me");

			if (response.IsSuccessStatusCode)
			{
				// Use cancellation token if available for better control
				var user = await response.Content.ReadFromJsonAsync<UserDto>();
				return user;
			}

			if (response.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("User is not authorized.");

			if (response.StatusCode == HttpStatusCode.NotFound)
				throw new KeyNotFoundException("User profile not found.");

			throw new HttpRequestException(
				$"Failed to fetch user profile. Status: {response.StatusCode}");
		}
		catch (HttpRequestException ex)
		{
			// Log network errors separately
			Console.WriteLine($"Network error fetching user: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching user: {ex.Message}");
			throw;
		}
	}
}
