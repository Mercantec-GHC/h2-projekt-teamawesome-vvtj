using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.Auth;

public partial class Login
{
	[Inject] 
	private IAuthService _authService { get; set; } = null!;
	[Inject] 
	private NavigationManager _navigation{ get; set; } = null!;
	private string _errorMessage = string.Empty;
	private LoginDto _loginModel = new();


	private async Task HandleLogin()
	{
		_errorMessage = string.Empty;
	
		var result = await _authService.LoginAsync(_loginModel.Email, _loginModel.Password);
		if (result)
		{
			_navigation.NavigateTo("/");
		}
		else
		{
			_errorMessage = "Invalid email or password.";
		}
	}
}
