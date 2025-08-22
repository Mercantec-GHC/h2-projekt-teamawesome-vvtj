using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	public class UserService : IUserService
	{
		private readonly AppDBContext _context;
		private readonly UserMapping _userMapping = new();

		public UserService(AppDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
		{
			var users = await _context.Users
				.Include(u => u.UserRole)
				.ToListAsync();

			var userDtos = users.Select(u => _userMapping.ToUserGetDto(u)).ToList();
			return userDtos;
		}

		public async Task<UserDto?> GetUserByIdAsync(int id)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Id == id);
			if (user == null)
			{
				return null;
			}

			var userDto = _userMapping.ToUserGetDto(user);
			return userDto;
		}

		public async Task<UserDto> GetUserByEmailAsync(string email)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				throw new KeyNotFoundException("User not found.");
			}
			var userDto = _userMapping.ToUserGetDto(user);
			return userDto;
		}

		public async Task<bool> DeleteUserByEmailAsync(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				return false;
			}
			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
