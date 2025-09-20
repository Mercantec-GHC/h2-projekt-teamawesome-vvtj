using Blazor.Models.ViewModels;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class UserProfile : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	private CustomAuthStateProvider _authState { get; set; } = default!;
	[Inject]
	protected PreloadService PreloadService { get; set; } = default!;
	private CurrentUserProfileViewModel? _userProfileVM { get; set; }
	protected override async Task OnInitializedAsync()
	{
		PreloadService.Show();

		var authState = await _authState.GetAuthenticationStateAsync();
		if (authState.User.Identity?.IsAuthenticated ?? false)
		{
			var user = await _apiService.GetCurrentUserInfoAsync();
			if (user != null)
			{
				_userProfileVM = user.ToUserInfoViewModel();
			}
		}

		PreloadService.Hide();
	}
}