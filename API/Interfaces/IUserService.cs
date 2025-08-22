using DomainModels.Dto.UserDto;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
		Task<UserDto?> GetUserByIdAsync(int id);
		Task<bool> DeleteUserByEmailAsync(string email);
        Task<UserGetDto> GetUserByEmailAsync(string email);
	}
}
