using DomainModels.Dto.UserProfileDto;
using DomainModels.Models;

namespace API.Interfaces;

public interface IUserInfoService
{
	Task<UserInfoGetDto?> GetByUserIdAsync(int id);
	Task<UserInfo?> UpdateUserInfoAsync(int id, UserInfoPutDto updatedInfo);
	Task<UserInfo?> CreateUserInfoAsync(UserInfoPostDto newInfo);
}