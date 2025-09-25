using DomainModels.Dto.AuthDto;
using DomainModels.Dto.UserDto;
using DomainModels.Models;

namespace API.Interfaces;

/// <summary>
///  Defines authentication and user account management operations for Blazor WebAssembly applications.
/// </summary>
public interface IAuthService
{
	/// <summary>
	/// Registers a new user account using the provided registration details.
	/// </summary>
	/// <param name="request">The registration information, including email, username, and password.</param>
	/// <returns>
	/// A <see cref="UserDto"/> containing the newly created user's details if registration succeeds; otherwise, <c>null</c>.
	/// </returns>
	Task<UserDto?> RegisterUserAsync(RegisterDto request);

	/// <summary>
	/// Authenticates a user and returns a JWT token if successful.
	/// </summary>
	/// <param name="request">The login credentials, typically email and password.</param>
	/// <returns>
	/// A <see cref="TokenResponseDto"/> containing the access and refresh tokens if authentication is successful; otherwise, <c>null</c>.
	/// </returns>
	Task<TokenResponseDto?> LoginUserAsync(LoginDto request);

	/// <summary>
	/// Creates a refresh token for session renewal, binding it to the user's IP address and device.
	/// Used to maintain user sessions securely in Blazor WebAssembly applications.
	/// </summary>
	/// <param name="ipAddress">The IP address from which the refresh token is requested.</param>
	/// <param name="device">A string identifying the user's device.</param>
	/// <returns>
	/// A <see cref="RefreshToken"/> object containing the token and associated metadata.
	/// </returns>
	Task<RefreshToken?> CreateRefreshTokenAsync(string ipAddress, string device);

	Task<TokenResponseDto?> RefreshTokenAsync(string token);
	/// <summary>
	/// Changes the password for a user identified by their email address.
	/// Intended for administrative password resets or recovery scenarios.
	/// </summary>
	/// <param name="userEmail">The email address of the user whose password will be changed.</param>
	/// <param name="newPassword">The new password to set for the user.</param>
	/// <returns>
	/// <c>true</c> if the password was changed successfully; otherwise, <c>false</c>.
	/// </returns>
	Task<bool> ChangeUserPasswordAsync(string userEmail, string newPassword);

	/// <summary>
	/// Allows a user to change their own password by providing their user ID, current password, and new password.
	/// </summary>
	/// <param name="userId">The unique identifier of the user requesting the password change.</param>
	/// <param name="currentPassword">The user's current password for verification.</param>
	/// <param name="newPassword">The new password to set.</param>
	/// <returns>
	/// <c>true</c> if the password was changed successfully; otherwise, <c>false</c>.
	/// </returns>
	Task<bool> ChangeOwnPasswordAsync(string userId, string currentPassword, string newPassword);
}
