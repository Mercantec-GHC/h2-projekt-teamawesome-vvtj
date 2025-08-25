using DomainModels.Dto.UserProfileDto;
using DomainModels.Models;

namespace API.Interfaces;

public interface IUserInfoService
{
	Task<UserInfoGetDto?> GetByUserEmailAsync(int id);
	Task<UserInfoPutDto?> UpdateUserInfoAsync(int id, UserInfo updatedInfo);
	Task<UserInfoPostDto?> CreateUserInfoAsync(UserInfo newInfo);
}