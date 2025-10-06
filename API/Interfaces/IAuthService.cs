using DomainModels.Dto.AuthDto;
using DomainModels.Dto.UserDto;
using DomainModels.Models;

namespace API.Interfaces;

public interface IAuthService
{
	Task<UserDto?> RegisterUserAsync(RegisterDto request);

	Task<TokenResponseDto?> LoginUserAsync(LoginDto request);

	Task<RefreshToken?> CreateRefreshTokenAsync(string ipAddress, string device);

	Task<TokenResponseDto?> RefreshTokenAsync(string token, string ipAddress, string device);

	Task<bool> ChangeUserPasswordAsync(string userEmail, string newPassword);

	Task<bool> ChangeOwnPasswordAsync(string userId, string currentPassword, string newPassword);
}
