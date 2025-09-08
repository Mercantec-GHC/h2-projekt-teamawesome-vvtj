using System.Net;
using System.Net.Http.Json;
using DomainModels.Dto.UserDto;

namespace Blazor.Services;

public partial class APIService
{
	public async Task<UserDto?> GetCurrentUserAsync()
	{
		try
		{
			// Ensure HttpClient is properly configured for credentials/cookies if needed
			var response = await GetAsync("api/Users/me");

			if (response.IsSuccessStatusCode)
			{
				var user = await response.Content.ReadFromJsonAsync<UserDto>();
				return user;
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				_logger.LogWarning("User profile not found.");
				return null;
			}

			_logger.LogError("Failed to fetch user profile. Status: {StatusCode}", response.StatusCode);
			return null;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error fetching user profile.");
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Unexpected error fetching user profile.");
			throw;
		}
	}
}
