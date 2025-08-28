namespace Blazor.Interfaces;

public interface IAuthService
{
	Task<bool> LoginAsync(string email, string password);
	Task<bool> RegisterAsync(string email, string userName, string password, string confirmPassword);
	Task LogoutAsync();
}