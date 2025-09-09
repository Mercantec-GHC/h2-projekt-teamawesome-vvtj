using DomainModels.Dto.UserProfileDto;

namespace API.Interfaces;

public interface IUserInfoService
{
	Task<UserInfoGetDto?> GetByUserIdAsync(int id);
	Task<UserInfoGetDto?> GetCurrentUserInfoAsync(string Id);
	Task<UserInfoGetDto?> UpdateUserInfoAsync(int userId, UserInfoPutDto updatedInfo);
}