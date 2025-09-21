using Blazor.Models.ViewModels;
using Blazor.Services;
using BlazorBootstrap;
using DomainModels.Dto.UserProfileDto;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class EditUserProfile : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	private CustomAuthStateProvider _authState { get; set; } = default!;
	[Inject]
	protected PreloadService PreloadService { get; set; } = default!;
	private CurrentUserProfileViewModel? _userProfileVM { get; set; } = new();
	[SupplyParameterFromForm(FormName = "EditInfoForm")]
	CurrentUserProfileViewModel EditInfoForm { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		PreloadService.Show();
		var currentUser = await _apiService.GetCurrentUserInfoAsync();
		PreloadService.Hide();
		if (currentUser != null)
		{
			_userProfileVM = currentUser.ToViewModel();
			EditInfoForm = currentUser.ToViewModel();
		}
	}

	private async Task HandleOnValidSubmit()
	{
		if (EditInfoForm != null)
		{
			var userInfoPutDto = new UserInfoPutDto();
			EditInfoForm.ToUserInfoPutDto(EditInfoForm, userInfoPutDto);

			PreloadService.Show();
			var success = await _apiService.UpdateUserInfoAsync(userInfoPutDto);

			PreloadService.Hide();

			StateHasChanged();
			if (success)
			{
				var updatedUser = await _apiService.GetCurrentUserInfoAsync();
				if (updatedUser != null)
				{
					EditInfoForm = updatedUser.ToViewModel();
				}
			}

		}
	}
}
