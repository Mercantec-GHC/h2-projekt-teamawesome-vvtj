using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserProfileDto;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides business logic for managing user profile information,
/// including retrieval and updates of user profile data.
/// </summary>

public class UserInfoService : IUserInfoService
{
	private readonly AppDBContext _context;
	private readonly UserInfoMapping _userInfoMapping = new();

	public UserInfoService(AppDBContext context)
	{
		_context = context;
	}

	/// <summary>
	/// Retrieves the profile information for a user by their unique user ID.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>
	/// A <see cref="UserInfoGetDto"/> containing the user's profile information if found; otherwise, <c>null</c>.
	/// </returns>
	public async Task<UserInfoGetDto?> GetByUserIdAsync(int id)
	{
		var userInfo = await _context.UserInfos.FirstOrDefaultAsync(ui => ui.UserId == id);
		if (userInfo == null)
		{
			return null;
		}
		return _userInfoMapping.ToUserInfoGetDto(userInfo);
	}

	/// <summary>
	/// Retrieves the profile information for the currently authenticated user.
	/// </summary>
	/// <param name="Id">The unique identifier of the current user as a string.</param>
	/// <returns>
	/// A <see cref="UserInfoGetDto"/> containing the current user's profile information if found; otherwise, <c>null</c>.
	/// </returns>
	public async Task<UserInfoGetDto?> GetCurrentUserInfoAsync(string Id)
	{
		var userInfo = await _context.UserInfos.FirstOrDefaultAsync(ui => ui.UserId.ToString() == Id);

		if (userInfo == null)
			return null;

		return _userInfoMapping.ToUserInfoGetDto(userInfo);
	}

	/// <summary>
	/// Updates the profile information for a specific user.
	/// </summary>
	/// <param name="userId">The unique identifier of the user whose profile is to be updated.</param>
	/// <param name="dto">The updated profile data.</param>
	/// <returns>
	/// A <see cref="UserInfoGetDto"/> containing the updated profile information if the update is successful; otherwise, <c>null</c>.
	/// </returns>
	public async Task<UserInfoGetDto?> UpdateUserInfoAsync(int userId, UserInfoPutDto dto)
	{
		var userInfo = await _context.UserInfos.FirstOrDefaultAsync(ui => ui.UserId == userId);
		if (userInfo == null)
		{
			return null;
		}

		_userInfoMapping.UpdateUserInfoFromDto(userInfo, dto);
		_context.UserInfos.Update(userInfo);

		await _context.SaveChangesAsync();
		return _userInfoMapping.ToUserInfoGetDto(userInfo);
	}
}
