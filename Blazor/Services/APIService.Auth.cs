using System.Net.Http.Json;
using Blazor.Models.Dto.Auth;

namespace Blazor.Services;

public partial class APIService
{
	public record LoginRequest(string Email, string Password);

	public record LoginResponse(string Token, DateTime ExpiresAt);

	public record RegisterRequest(string Email, string Password);

	public class AuthResult
	{
		public bool Success { get; init; }
		public string? Error { get; init; }
		public LoginResponse? Payload { get; init; }

		public static AuthResult Failure(string error) => new() { Success = false, Error = error };
		public static AuthResult SuccessResult(LoginResponse payload) => new() { Success = true, Payload = payload };
	}

	public async Task<bool> LoginAsync(LoginRequestDto loginRequest)
	{
		var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

		if (!response.IsSuccessStatusCode)
			return false;

		var result = await response.Content.ReadFromJsonAsync<LoginDto>();

		if (result == null || string.IsNullOrWhiteSpace(result.Token))
			return false;

		await _authStateProvider.MarkUserAsAuthenticated(result.Token);

		return true;
	}

	public async Task LogoutAsync()
	{
		await _authStateProvider.MarkUserAsLoggedOut();
	}

	public async Task<bool> RegisterAsync(RegisterDto registerDto)
	{
		var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerDto);
		return response.IsSuccessStatusCode;
	}
}
