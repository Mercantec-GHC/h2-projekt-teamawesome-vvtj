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
	public async Task<UserInfoGetDto?> UpdateUserInfoAsync(UserInfoPutDto userInfo)
	{
		try
		{
			var response = await PutAsJsonAsync("api/UserInfo/update-my-profile", userInfo);
			if (response.IsSuccessStatusCode)
			{
				var userInfoResponse = await response.Content.ReadFromJsonAsync<UserInfoGetDto>();
				return userInfoResponse;
			}
			_logger.LogError("Failed to update user profile. Status: {StatusCode}", response.StatusCode);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error updating user profile.");
			return null;
		}
	}

	public async Task<UserInfoGetDto?> CreateUserInfoAsync(UserInfoPostDto userInfo)
	{
		try
		{
			var response = await PostAsJsonAsync("api/UserInfo/create-my-profile", userInfo);
			if (response.IsSuccessStatusCode)
			{
				var userInfoResponse = await response.Content.ReadFromJsonAsync<UserInfoGetDto>();
				return userInfoResponse;
			}
			_logger.LogError("Failed to create user profile. Status: {StatusCode}", response.StatusCode);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error creating user profile.");
			return null;
		}
	}
}