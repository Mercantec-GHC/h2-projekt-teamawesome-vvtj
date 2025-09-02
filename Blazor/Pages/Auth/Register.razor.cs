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
	private RegisterDto _registerModel = new();

	private async Task HandleRegister()
	{
		_errorMessage = string.Empty;

		if (_registerModel.Password != _registerModel.ConfirmPassword)
		{
			_errorMessage = "Passwords do not match.";
			return;
		}

		try
		{
			var emailExists = await _authService.RegisterAsync(
				_registerModel.Email,
				_registerModel.Username,
				_registerModel.Password,
				_registerModel.ConfirmPassword
			);


			if (!emailExists)
			{
				_errorMessage = "Account with this email already exists in our system.";
				return;
			}
			else
			{
				_navigation.NavigateTo("/login");
			}
		}
		catch (Exception ex)
		{
			_errorMessage = $"An error occurred: {ex.Message}";
			return;
		}
	}
}
