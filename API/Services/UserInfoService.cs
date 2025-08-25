using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserProfileDto;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;



public class UserInfoService : IUserInfoService
{
	private readonly AppDBContext _context;
	private readonly UserInfoMapping _userInfoMapping = new();

	public UserInfoService(AppDBContext context)
	{
		_context = context;
	}

	public async Task<UserInfoGetDto?> GetByUserIdAsync(int id)
	{
		var userInfo = await _context.UserInfos.FirstOrDefaultAsync(ui => ui.UserId == id);
		if (userInfo == null)
		{
			return null;
		}
		return _userInfoMapping.ToUserInfoGetDto(userInfo);
	}

	public async Task<UserInfo?> UpdateUserInfoAsync(int userId, UserInfoPutDto updatedInfo)
	{
		var existingUser = await _context.UserInfos
			.FirstOrDefaultAsync(ui => ui.UserId == userId);

		if (existingUser == null)
		{
			return null;
		}

		_userInfoMapping.UpdateUserInfoFromDto(existingUser, updatedInfo);
		return await _context.SaveChangesAsync() > 0 ? existingUser : null;
	}

	public async Task<UserInfo?> CreateUserInfoAsync(UserInfoPostDto newInfo)
	{
		var userInfo = _userInfoMapping.FromDtoToUserInfo(newInfo);
		_context.UserInfos.Add(userInfo);
		return await _context.SaveChangesAsync() > 0 ? userInfo : null;
	}
}
