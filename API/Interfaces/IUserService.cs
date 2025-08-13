using DomainModels.Dto;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserDto userDto);
    }
}
