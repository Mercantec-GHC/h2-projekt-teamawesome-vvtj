using DomainModels.Dto.UserDto;

namespace API.Interfaces;

public interface IUserService
{
	Task<IEnumerable<UserDto>> GetAllUsersAsync();
	Task<UserDto?> GetUserByIdAsync(int id);
	Task<UserDtoUnsafe?> GetUserFromTokenAsync(string userId);
	Task<bool> DeleteUserByEmailAsync(string email);
	Task<UserDto> GetUserByEmailAsync(string email);
}
