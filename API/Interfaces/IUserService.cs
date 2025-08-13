using DomainModels.Dto.UserDto;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserGetDto>> GetAllUsersAsync();
		Task<UserGetDto?> GetUserByIdAsync(int id);
        Task<UserPostDto?> CreateUserAsync(UserPostDto userDto);
        Task<bool> UpdateUserAsync(int id, UserPostDto userDto);
		Task<bool> DeleteUserByEmailAsync(string email);
	}
}
