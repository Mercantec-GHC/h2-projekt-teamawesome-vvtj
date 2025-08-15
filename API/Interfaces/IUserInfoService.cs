using DomainModels.Models;

namespace API.Interfaces;

public interface IUserInfoService
{
	//Create and delete are not needed for UserInfo, as it is tied to User.
	Task<UserInfo?> GetByUserIdAsync(int userId);
	Task<UserInfo?> UpdateUserInfoAsync(int userId, UserInfo updatedInfo);
}