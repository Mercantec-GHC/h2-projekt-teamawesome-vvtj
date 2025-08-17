using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Mapping;
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

		public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync()
		{
			var users = await _context.Users
				.Include(u => u.UserRole)
				.ToListAsync();

			var userDtos = users.Select(u => _userMapping.ToUserGetDto(u)).ToList();
			return userDtos;
		}

		public async Task<UserGetDto?> GetUserByIdAsync(int id)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Id == id);
			if (user == null)
			{
				return null;
			}

			var userDto = _userMapping.ToUserGetDto(user);
			return userDto;
		}

		public async Task<bool?> CreateUserAsync(UserPostDto dto)
		{
			var newUser = _userMapping.ToUserFromDto(dto); // Use the instance to call the method

			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> UpdateUserAsync(UserPostDto dto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user == null)
			{
				return false;
			}
			user.Email = dto.Email;
			user.UserName = dto.UserName;
			user.HashedPassword = dto.Password;
			user.Salt = dto.Password;
			await _context.SaveChangesAsync();
			return true;
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
