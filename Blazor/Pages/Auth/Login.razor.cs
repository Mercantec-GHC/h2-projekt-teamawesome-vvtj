using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.Auth;

public partial class Login
{
	[Inject] 
	private IAuthService _authService { get; set; } = null!;
	[Inject] 
	private NavigationManager _navigation{ get; set; } = null!;
	[Inject]
	protected PreloadService PreloadService { get; set; } = default!;

	private string _errorMessage = string.Empty;
	private UserLoginDto _loginModel = new();

	private async Task HandleLogin()
	{
		PreloadService.Show();
		_errorMessage = string.Empty;
		var result = await _authService.LoginAsync(_loginModel.Username, _loginModel.Password, _loginModel.RememberMe);
		if (result)
		{
			_navigation.NavigateTo("/user/account");

		}
		else
		{
			_errorMessage = "Invalid email or password.";
		}
		PreloadService.Hide();
	}
}
