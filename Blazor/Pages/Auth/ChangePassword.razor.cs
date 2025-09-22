using Blazor.Interfaces;
using Blazor.Models.ViewModels;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.Auth;

public partial class ChangePassword : ComponentBase
{
	[Inject]
	private IAuthService _authService { get; set; } = default!;
	[Inject]
	private NavigationManager _navigation { get; set; } = default!;
	[Inject]
	private APIService _apiService { get; set; } = default!;
	[Inject]
	PreloadService _preloadService { get; set; } = default!;
	private ChangePasswordViewModel _changePasswordVM;
	private string _errorMessage = string.Empty;

	protected override void OnInitialized()
	{
		_changePasswordVM = new ChangePasswordViewModel();
	}

	private async Task HandleChangedPassword()
	{
		try
		{
			_preloadService.Show();
			_errorMessage = string.Empty;
			var result = await _authService.ChangePasswordAsync(_changePasswordVM.NewPassword, _changePasswordVM.ConfirmNewPassword);
			if (result)
			{
				await _authService.LogoutAsync(); //to clear token and auth state
				_navigation.NavigateTo("/login");
			}
			else
			{
				_errorMessage = "Failed to change password.";
			}
		}
		catch (Exception ex)
		{
			_errorMessage = $"An error occurred: {ex.Message}";
		}
		finally
		{
			_preloadService.Hide();
		}
	}

	private void CancelChangePassword()
	{
		_navigation.NavigateTo("/user/account", forceLoad:true);
	}
}
