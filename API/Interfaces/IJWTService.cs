using DomainModels.Models;
using static API.Services.ActiveDirectoryService;

namespace API.Interfaces;

/// <summary>
/// Service for creating JWT tokens for application and AD users.
/// </summary>
public interface IJWTService
{
	/// <summary>
	/// Creates a JWT token for a standard application user.
	/// </summary>
	/// <param name="user">The application user.</param>
	/// <returns>A JWT token string.</returns>
	string CreateToken(User user);

	/// <summary>
	/// Creates a JWT token for an Active Directory user.
	/// </summary>
	/// <param name="adUser">The AD user information.</param>
	/// <returns>A JWT token string.</returns>
	string CreateToken(ADUserInfo adUser);
}
