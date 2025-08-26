using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserProfileDto;
using DomainModels.Mapping;
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
