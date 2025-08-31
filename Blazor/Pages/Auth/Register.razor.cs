using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.Auth;

public partial class Register
{
	[Inject]
	private IAuthService _authService { get; set; } = null!;
	[Inject]
	private NavigationManager _navigation { get; set; } = null!;
	private string _errorMessage = string.Empty;
	private string _successMessage = string.Empty;
	private RegisterDto _registerModel = new();

	private async Task HandleRegister()
	{
		_errorMessage = string.Empty;
		_successMessage = string.Empty;

		if (_registerModel.Password != _registerModel.ConfirmPassword)
		{
			_errorMessage = "Passwords do not match.";
			return;
		}

		var result = await _authService.RegisterAsync(
		_registerModel.Email,
		_registerModel.Username,
		_registerModel.Password,
		_registerModel.ConfirmPassword
		);

		if (result)
		{
			_successMessage = "Registration successful! You can now log in.";
			_navigation.NavigateTo("/login");
		}
		else
		{
			_errorMessage = "Registration failed. Please try again.";
		}
	}
}
