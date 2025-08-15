using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace API.Services
{
	public class UserService : IUserService
	{
		private readonly AppDBContext _context;
		private readonly UserMapping userMapping = new();

		public UserService(AppDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync()
		{
			var users = await _context.Users.Include(u => u.UserRole).ToListAsync();
			return users.Select(u => new UserGetDto
			{
				Id = u.Id,
				Email = u.Email,
				Role = u.UserRole?.RoleName ?? string.Empty
			});
		}

		public async Task<UserGetDto?> GetUserByIdAsync(int id)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Id == id);
			if (user == null)
			{
				return null;
			}

			return new UserGetDto
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				Role = user.UserRole.RoleName ?? string.Empty,
				LastLogin = (DateTime)user.LastLogin
			};
		}

		public async Task<bool?> CreateUserAsync(UserPostDto dto)
		{
			var newUser = userMapping.ToUserFromDto(dto); // Use the instance to call the method

			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> UpdateUserAsync(UserPostDto dto)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Email == dto.Email);
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
