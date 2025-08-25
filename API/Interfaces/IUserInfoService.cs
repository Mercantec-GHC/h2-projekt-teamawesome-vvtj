using DomainModels.Dto.UserProfileDto;
using DomainModels.Models;

namespace API.Interfaces;

public interface IUserInfoService
{
	Task<UserInfoGetDto?> GetByUserIdAsync(int id);
	Task<UserInfoGetDto?> UpdateUserInfoAsync(int userId, UserInfoPutDto updatedInfo);
}