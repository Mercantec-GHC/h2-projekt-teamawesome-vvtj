using Blazor.Helpers;
using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Pages.Auth;

public partial class Login
{
	[Inject] 
	private IAuthService _authService { get; set; } = null!;
	[Inject] 
	private NavigationManager _navigation{ get; set; } = null!;
	[Inject]
	private IJSRuntime JSRuntime { get; set; } = null!;
	private string _errorMessage = string.Empty;
	private UserLoginDto _loginModel = new();
	private JwtHelper _jwlHelper = new();	


	private async Task HandleLogin()
	{
		_errorMessage = string.Empty;
		var result = await _authService.LoginAsync(_loginModel.Email, _loginModel.Password, _loginModel.RememberMe);
		if (result)
		{
			_navigation.NavigateTo("/user/account");
			var token = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

			if (!string.IsNullOrEmpty(token))
			{
				var cleanToken = token.Trim('"');
				var claims = _jwlHelper.GetClaimsFromJwt(cleanToken);

				var roleClaim = claims.FirstOrDefault(c =>
					   c.Type == "role" ||
					   c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

				if (roleClaim?.Value == "Admin")
				{
					await RequestNotificationSubscriptionAsync();
				}
			}

		}
		else
		{
			_errorMessage = "Invalid email or password.";
		}
	}

	private async Task RequestNotificationSubscriptionAsync()
	{
		var subscription = await JSRuntime.InvokeAsync<NotificationSubscriptionDto>(
			"blazorPushNotifications.requestSubscription");

		if (subscription != null)
		{
			var result = await ApiService.SubscribeToPushNotificationsAsync(subscription);
		}
	}
}
