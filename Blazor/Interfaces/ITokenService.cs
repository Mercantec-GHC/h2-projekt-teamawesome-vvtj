namespace Blazor.Interfaces;

public interface ITokenService
{
	Task<string?> RefreshAccessTokenAsync();
	Task<string?> GetTokenAsync();
	Task SaveTokenAsync(string token, bool rememberMe);
}
