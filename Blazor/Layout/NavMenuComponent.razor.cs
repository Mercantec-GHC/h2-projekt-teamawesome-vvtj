using Blazor.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Layout;

public partial class NavMenuComponent
{
	[Inject]
	public IAuthService AuthService { get; set; } = default!;
	[Inject]
	public NavigationManager Navigation { get; set; } = default!;

	private async Task HandleLogout()
	{
		await AuthService.LogoutAsync();
		StateHasChanged();
		Navigation.NavigateTo("/login");
	}
}
