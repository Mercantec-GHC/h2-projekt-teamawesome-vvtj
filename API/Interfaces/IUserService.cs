using DomainModels.Dto.UserDto;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserGetDto>> GetAllUsersAsync();
		Task<UserGetDto?> GetUserByIdAsync(int id);
        Task<bool?> CreateUserAsync(UserPostDto userDto);
        Task<bool> UpdateUserAsync(UserPostDto dto);
		Task<bool> DeleteUserByEmailAsync(string email);
	}
}
