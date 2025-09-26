using DomainModels.Models;
using static API.Services.ActiveDirectoryService;

namespace API.Interfaces;

/// <summary>
/// Provides functionality for generating JWT access tokens and refresh tokens for both application and Active Directory users.
/// Used for authentication and session management in Blazor WebAssembly applications.
/// </summary>
public interface IJWTService
{
	/// <summary>
	/// Generates a JWT access token for a standard application user.
	/// The token includes user claims and roles for secure authentication.
	/// </summary>
	/// <param name="user">The application user for whom the token is generated.</param>
	/// <returns>
	/// A JWT token string representing the authenticated user.
	/// </returns>
	string CreateToken(User user);

	/// <summary>
	/// Generates a JWT access token for an Active Directory user.
	/// The token includes AD user claims and group memberships for secure authentication.
	/// </summary>
	/// <param name="adUser">The Active Directory user information.</param>
	/// <returns>
	/// A JWT token string representing the authenticated AD user.
	/// </returns>
	string CreateToken(ADUserInfo adUser);
}
