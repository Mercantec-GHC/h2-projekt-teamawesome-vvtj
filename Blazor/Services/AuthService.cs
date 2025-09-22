using System.Net.Http.Json;
using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DomainModels.Dto.UserDto;

namespace Blazor.Services;

public class AuthService : IAuthService

{
	private readonly APIService _apiService;
	private readonly ILocalStorageService _localStorage;
	private readonly ISessionStorageService _sessionStorage;
	private readonly CustomAuthStateProvider _authStateProvider;
	private const string _tokenKey = "authToken";

	public AuthService(APIService apiService, ILocalStorageService localStorage, ISessionStorageService sessionStorage, CustomAuthStateProvider authStateProvider)
	{
		_apiService = apiService;
		_sessionStorage = sessionStorage;
		_localStorage = localStorage;
		_authStateProvider = authStateProvider;
	}

	public async Task<bool> LoginAsync(string email, string password, bool remember)
	{
		// Create the login DTO with data for a login
		var loginDto = new UserLoginDto
		{
			Email = email,
			Password = password,
			RememberMe = remember
		};

		// Send the login request to the API
		var response = await _apiService.PostAsJsonAsync("api/Auth/login", loginDto);

		if (!response.IsSuccessStatusCode)
			return false;

		// Read the token from the response
		var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
		var token = result?.Token;

		if (string.IsNullOrWhiteSpace(token))
			return false;
		var cleanToken = token.Trim('"');

		// Store the token in local storage if remember me is checked, otherwise in session storage
		if (remember)
		{
			await _localStorage.SetItemAsync(_tokenKey, cleanToken);
			await _sessionStorage.RemoveItemAsync(_tokenKey);
		}

		else
		{
			await _sessionStorage.SetItemAsync(_tokenKey, cleanToken);
			await _localStorage.RemoveItemAsync(_tokenKey);
		}

		// Notify Blazor that the user is authenticated
		_authStateProvider.NotifyUserAuthentication(cleanToken);

		// Set the token in the HttpClient for future requests
		_apiService.SetBearerToken(cleanToken);

		// Send a message to admin dashboard to about the login event
		var message = $"User {loginDto.Email} has just logged in";
		await _apiService.SendNotificationAsync(message);

		return true;
	}

	public async Task<bool> RegisterAsync(string email, string userName, string password, string confirmPassword)
	{
		// Create the register DTO with data for a new user
		var registerDto = new UserRegisterDto
		{
			Email = email,
			Username = userName,
			Password = password,
			ConfirmPassword = confirmPassword
		};

		// Calls backend; Send the registration request to the API
		var response = await _apiService.PostAsJsonAsync("api/Auth/register", registerDto);

		// If registration failed, return false
		if (!response.IsSuccessStatusCode)
			return false;

		// Optionally, read the created user info from the response
		var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
		Console.WriteLine($"New user created: {createdUser?.UserName}");

		return response.IsSuccessStatusCode;
	}

	public async Task LogoutAsync()
	{
		await _localStorage.RemoveItemAsync(_tokenKey);
		await _sessionStorage.RemoveItemAsync(_tokenKey);
		_apiService.RemoveBearerToken();
		_authStateProvider.NotifyUserLogout();
	}

	public async Task <bool> ChangePasswordAsync(string newPassword, string confirmNewPassword)
	{
		if (newPassword != confirmNewPassword)
			return false;

		var changePasswordDto = new ChangePasswordDto
		{
			NewPassword = newPassword,
			ConfirmPassword = confirmNewPassword
		};

		var response = await _apiService.PostAsJsonAsync("api/Auth/change-own-password", changePasswordDto);
		return response.IsSuccessStatusCode;
	}

}
