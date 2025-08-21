using DomainModels.Dto.UserDto;

namespace API.Interfaces;

/// <summary>
/// Service for user authentication and registration.
/// </summary>
public interface IAuthService
{
	/// <summary>
	/// Registers a new user.
	/// </summary>
	/// <param name="request">Registration data.</param>
	/// <returns>User details if registration is successful; otherwise, null.</returns>
	Task<UserGetDto?> RegisterUserAsync(RegisterDto request);

	/// <summary>
	/// Authenticates a user and returns a JWT token if successful.
	/// </summary>
	/// <param name="request">Login credentials.</param>
	/// <returns>JWT token string if login is successful; otherwise, null.</returns>
	Task<string?> LoginUserAsync(LoginDto request);
}
