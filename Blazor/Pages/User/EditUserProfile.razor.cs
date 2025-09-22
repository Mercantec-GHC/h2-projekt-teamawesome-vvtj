using Blazor.Models.ViewModels;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class EditUserProfile : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	protected PreloadService PreloadService { get; set; } = default!;
	[Inject]
	private NavigationManager _navigation { get; set; } = default!;
	private CurrentUserProfileViewModel? _editInfoForm { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		PreloadService.Show();
		var currentUser = await _apiService.GetCurrentUserInfoAsync();
		PreloadService.Hide();
		if (currentUser != null)
		{
			_editInfoForm = currentUser.ToUserInfoViewModel();
		}
	}

	private async Task HandleOnValidSubmit()
	{
		if (_editInfoForm != null)
		{
			var userInfoPutDto = _editInfoForm.ToUserInfoPutDto();

			PreloadService.Show();
			var success = await _apiService.UpdateUserInfoAsync(userInfoPutDto);
			PreloadService.Hide();

			if (success != null)
			{
				var updatedUser = await _apiService.GetCurrentUserInfoAsync();
				if (updatedUser != null)
				{
					_navigation.NavigateTo("/user/profile", forceLoad: true);
				}
			}

		}
	}

	private void CancelEdit()
	{
		_navigation.NavigateTo("/user/profile");
	}
}