using Blazor.Models.ViewModels;
using Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class UserProfile : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	private CustomAuthStateProvider _authState { get; set; } = default!;
	[Parameter]
	public string? UserId { get; set; }
	private CurrentUserProfileViewModel? _userProfileVM { get; set; }
	protected override async Task OnInitializedAsync()
	{
		var authState = await _authState.GetAuthenticationStateAsync();

		if (authState.User.Identity?.IsAuthenticated ?? false)
		{
			var user = await _apiService.GetCurrentUserInfoAsync();

			if (user != null)
			{
				_userProfileVM = user.ToViewModel();
			}
		}
		
	}
}