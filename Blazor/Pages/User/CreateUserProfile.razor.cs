using Blazor.Models.ViewModels;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class CreateUserProfile : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	private PreloadService _preloadService { get; set; } = default!;
	[Inject]
	private NavigationManager _navigation { get; set; } = default!;
	private CurrentUserProfileViewModel? _createInfoForm { get; set; } = new();
	private bool isLoading = false;
	private string? errorMessage;
	private string? successMessage;

	protected override async Task OnInitializedAsync()
	{
		var existingUser = await _apiService.GetCurrentUserInfoAsync();
		if (existingUser != null)
		{
			_navigation.NavigateTo("/user/profile", forceLoad: true);
			return;
		}
		isLoading = false;
	}
	private async Task HandleSubmit()
	{
		errorMessage = null;
		successMessage = null;
		_preloadService.Show();

		try
		{
			var userInfoPostDto = _createInfoForm?.ToUserInfoPostDto();

			var createdUser = await _apiService.CreateUserInfoAsync(userInfoPostDto!);
			if(createdUser != null)
			{
				successMessage = "Profile created successfully!";
				_createInfoForm = createdUser.ToUserInfoViewModel();

				errorMessage = null;
				_navigation.NavigateTo("/user/profile", forceLoad: true);
			}
			else
			{
				errorMessage = "Failed to create profile. Please try again.";
				successMessage = null;
			}
		}
		catch (HttpRequestException httpEx)
		{
			errorMessage = $"Network error while creating profile. Try again later. {httpEx.Message}";
			successMessage = null;
		}
		catch (Exception ex)
		{
			errorMessage = $"Unexpected error: {ex.Message}";
			successMessage = null;
		}
		finally
		{
			isLoading = false;
			_preloadService.Hide();
			StateHasChanged();
		}

	}

	private void CancelCreate()
	{
		_navigation.NavigateTo("/");
	}

}
