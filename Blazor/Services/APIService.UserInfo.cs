using System.Net;
using System.Net.Http.Json;
using DomainModels.Dto.UserProfileDto;

namespace Blazor.Services;

public partial class APIService
{
	public async Task<UserInfoGetDto?> GetCurrentUserInfoAsync()
	{
		try
		{
			var response = await GetAsync($"api/UserInfo/info");
			if (response.IsSuccessStatusCode)
			{
				var userInfo = await response.Content.ReadFromJsonAsync<UserInfoGetDto>();
				return userInfo;
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				_logger.LogWarning("User profile not found.");
				return null;
			}

			_logger.LogError("Failed to fetch user profile. Status: {StatusCode}", response.StatusCode);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error fetching user profile.");
			return null;
		}
	}
}