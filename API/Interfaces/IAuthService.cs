using DomainModels.Dto.UserDto;
using DomainModels.Models;

namespace API.Interfaces;

public interface IAuthService
{
	Task<User?> RegisterUserAsync(RegisterDto request);
	Task<string?> LoginUserAsync(LoginDto request);
}
