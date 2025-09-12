using Blazor.Models.ViewModels;
using Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.User;

public partial class UserAccount : ComponentBase
{
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	private CustomAuthStateProvider _authState { get; set; } = default!;
	private CurrentUserAccountViewModel? _currentUserVM;
	protected override async Task OnInitializedAsync()
	{
		var authState = await _authState.GetAuthenticationStateAsync();

		if (authState.User.Identity?.IsAuthenticated ?? false)
		{
			var user = await _apiService.GetCurrentUserAsync();
			if (user != null)
			{
				_currentUserVM = user.ToViewModel();
			}
		}
	}
}
