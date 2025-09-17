using Blazor.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Layout;

public partial class NavMenuComponent
{
	[Inject]
	public NavigationManager Navigation { get; set; } = default!;
	public UserMenuComponent UserMenu { get; set; } = default!;

	private async Task OnLoginClicked()
	{
		Navigation.NavigateTo("/login");
	}
}
