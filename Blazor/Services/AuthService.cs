using System.Net.Http.Json;
using Blazor.Interfaces;
using Blazored.LocalStorage;

namespace Blazor.Services;

public class AuthService : IAuthService

{
	private readonly HttpClient _httpClient;
	private readonly ILocalStorageService _localStorage;
	private const string _tokenKey = "token";

	public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
	{
		_httpClient = httpClient;
		_localStorage = localStorage;
	}

	public async Task<bool> LoginAsync (string email, string password)
	{
		var loginRequest = new { Email = email, Password = password };

		var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);

		if (!response.IsSuccessStatusCode)
			return false;

		var token = await response.Content.ReadAsStringAsync();
		await _localStorage.SetItemAsync(_tokenKey, token.Trim('"')); 

		return true;
	}

	public async Task<bool> RegisterAsync(string email, string userName, string password, string confirmPassword)
	{
		var registerRequest = new { Email = email, UserName = userName, Password = password, ConfirmPassword = confirmPassword };
		var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerRequest);
		return response.IsSuccessStatusCode;
	}

	public async Task LogoutAsync()
	{
		await _localStorage.RemoveItemAsync(_tokenKey);
	}

}