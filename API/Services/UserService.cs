using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	public class UserService : IUserService
	{
		private readonly AppDBContext _context;
		public UserService(AppDBContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync()
		{
			var users = await _context.Users.ToListAsync();
			return users.Select(u => new UserGetDto
			{
				Id = u.Id,
				Email = u.Email,
				Role = u.UserRole.RoleName
			});
		}

		public async Task<UserGetDto?> GetUserByIdAsync(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return null;
			}
			return new UserGetDto
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				Role = user.UserRole.RoleName,
				LastLogin = user.LastLogin
			};
		}

		public async Task<UserPostDto?> CreateUserAsync(UserPostDto dto)
		{
			var newUser = new User
			{
				UserName = dto.UserName,
				Email = dto.Email,
				HashedPassword = dto.Password,
				Salt = dto.Password,
				PasswordBackdoor = dto.Password, // Only for educational purposes, not in the final product!
				UserRoleId = dto.UserRoleId,
				LastLogin = DateTime.UtcNow
			};
			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();
			return new UserPostDto
			{
				Id = newUser.Id,
				Email = newUser.Email,
				UserName = newUser.UserName,
				UserRoleId = newUser.UserRoleId
			};
		}

		public async Task<bool> UpdateUserAsync(int id, UserPostDto dto)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return false;
			}
			user.Email = dto.Email;
			user.UserName = dto.UserName;
			user.HashedPassword = dto.Password;
			user.Salt = dto.Password;
			user.UserRoleId = dto.UserRoleId;
			_context.Users.Update(user);
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
