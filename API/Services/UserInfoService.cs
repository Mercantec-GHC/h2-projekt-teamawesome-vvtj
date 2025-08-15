using API.Data;
using API.Interfaces;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;



public class UserInfoService : IUserInfoService
{
	private readonly AppDBContext _context;

	public UserInfoService(AppDBContext context)
	{
		_context = context;
	}

	public async Task<UserInfo?> GetByUserIdAsync(int userId)
	{
		return await _context.UserInfos
			.FirstOrDefaultAsync(ui => ui.UserId == userId);
	}

	public async Task<UserInfo?> UpdateUserInfoAsync(int userId, UserInfo updatedInfo)
	{
		var existing = await _context.UserInfos
			.FirstOrDefaultAsync(ui => ui.UserId == userId);

		if (existing == null)
		{
			return null;
		}

		existing.FirstName = updatedInfo.FirstName;
		existing.LastName = updatedInfo.LastName;
		existing.Address = updatedInfo.Address;
		existing.PostalCode = updatedInfo.PostalCode;
		existing.City = updatedInfo.City;
		existing.Country = updatedInfo.Country;
		existing.PhoneNumber = updatedInfo.PhoneNumber;

		await _context.SaveChangesAsync();
		return existing;
	}
}
