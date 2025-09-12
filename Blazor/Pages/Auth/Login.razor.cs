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
	[Inject]
	private CustomAuthStateProvider _customAuthStateProvider { get; set; }

	private string _errorMessage = string.Empty;
	private UserLoginDto _loginModel = new();

	private async Task HandleLogin()
	{
		_errorMessage = string.Empty;
		var result = await _authService.LoginAsync(_loginModel.Email, _loginModel.Password, _loginModel.RememberMe);
		if (result)
		{
			_navigation.NavigateTo("/user/account");

			// If the user is an admin, request notification subscription
			var authState = await _customAuthStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			if (user.IsInRole("Admin"))
			{
				await RequestNotificationSubscriptionAsync();
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
