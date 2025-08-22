using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	/// <summary>
	/// Service for managing user-related operations.
	/// </summary>
	public class UserService : IUserService
	{
		private readonly AppDBContext _context;
		private readonly UserMapping _userMapping = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="UserService"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		public UserService(AppDBContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Retrieves all users from the database.
		/// </summary>
		/// <returns>A collection of <see cref="UserDto"/> objects.</returns>
		public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
		{
			var users = await _context.Users
				.Include(u => u.UserRole)
				.ToListAsync();

			var userDtos = users.Select(u => _userMapping.ToUserDto(u)).ToList();
			return userDtos;
		}

		/// <summary>
		/// Retrieves a user by their unique identifier.
		/// </summary>
		/// <param name="id">The user's unique identifier.</param>
		/// <returns>A <see cref="UserDto"/> if found; otherwise, <c>null</c>.</returns>
		public async Task<UserDto?> GetUserByIdAsync(int id)
		{
			var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Id == id);
			if (user == null)
			{
				return null;
			}

			var userDto = _userMapping.ToUserDto(user);
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
