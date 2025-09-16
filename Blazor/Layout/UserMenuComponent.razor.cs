using Blazor.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Layout;

public partial class UserMenuComponent
{
	[Inject]
	public IAuthService AuthService { get; set; } = default!;
	[Inject]
	public NavigationManager Navigation { get; set; } = default!;
	[Parameter]
	public EventCallback OnLoginClicked { get; set; }
	public bool IsUserMenuVisible { get; set; }
	private async Task HandleLogout()
	{
		await AuthService.LogoutAsync();
		StateHasChanged();
		Navigation.NavigateTo("/login");
	}
	public async Task HandleLogin()
	{
		await OnLoginClicked.InvokeAsync(null);
		StateHasChanged();
	}
}
