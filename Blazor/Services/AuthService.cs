using System.Net.Http.Json;
using Blazor.Interfaces;
using Blazor.Models.Dto.Auth;
using Blazor.Models.Dto.User;
using Blazored.LocalStorage;

namespace Blazor.Services;

public class AuthService : IAuthService

{
	private readonly APIService _apiService;
	private readonly ILocalStorageService _localStorage;
	private readonly CustomAuthStateProvider _authStateProvider;
	private const string _tokenKey = "token";

	public AuthService(APIService apiService, ILocalStorageService localStorage, CustomAuthStateProvider authStateProvider)
	{
		_apiService = apiService;
		_localStorage = localStorage;
		_authStateProvider = authStateProvider;
	}

	public async Task<bool> LoginAsync (string email, string password)
	{
		// Create the login DTO with data for a login
		var loginDto = new LoginDto
		{
			Email = email,
			Password = password
		};

		// Send the login request to the API
		var response = await _apiService.PostAsJsonAsync("api/Auth/login", loginDto);

		if (!response.IsSuccessStatusCode)
			return false;

		// Read the token from the response
		var token = await response.Content.ReadAsStringAsync();

		// Store the token in local storage
		await _localStorage.SetItemAsync(_tokenKey, token.Trim('"'));

		// Notify Blazor that the user is authenticated
		_authStateProvider.NotifyUserAuthentication(token.Trim('"'));

		// Set the token in the HttpClient for future requests
		_apiService.SetBearerToken(token.Trim('"'));

		return true;
	}

	public async Task<bool> RegisterAsync(string email, string userName, string password, string confirmPassword)
	{
		// Create the register DTO with data for a new user
		var registerDto = new RegisterDto
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

		_authStateProvider.NotifyUserLogout();
		_apiService.RemoveBearerToken();
	}

}