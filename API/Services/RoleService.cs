using API.Data;
using API.Interfaces;
using DomainModels.Dto.RoleDto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class RoleService : IRoleService
{
	private readonly AppDBContext _context;
	private readonly RoleMapping _roleMapping = new();
	public RoleService(AppDBContext context)
	{
		_context = context;
	}
	public async Task<IEnumerable<RoleDetailsDto>> GetAllRolesAsync()
	{
		var roles = await _context.Roles.ToListAsync();

		var roleDtos = roles.Select(r => _roleMapping.ToRoleDto(r)).ToList();
		return roleDtos;
	}
	public async Task<RoleDetailsDto?> GetRoleByIdAsync(int id)
	{
		var role = await _context.Roles.FindAsync(id);
		if (role == null)
		{
			return null;
		}

		var roleDto = _roleMapping.ToRoleDto(role);
		return roleDto;
	}
	public async Task<IEnumerable<UserDto>> GetUsersByRoleIdAsync(int roleId)
	{
		var users = await _context.Users
			.Where(u => u.UserRoleId == roleId)
			.ToListAsync();

		if (users == null || !users.Any())
		{
			return Enumerable.Empty<UserDto>();
		}

		return users.Select(u => new UserDto
		{
			Id = u.Id,
			UserName = u.UserName,
			Email = u.Email,
			UserRole = u.UserRole.RoleName.ToString(),
		});
	}

	/// <summary>
	/// Assigns a role to a user and returns the updated user as UserDto.
	/// </summary>
	/// <param name="dto">Assignment details.</param>
	/// <param name="userId">User ID.</param>
	/// <param name="roleId">Role ID.</param>
	/// <returns>Updated UserDto.</returns>
	public async Task<UserDto> AssignRoleToUserAsync(int userId, RoleEnum newRole)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
		if (user == null)
			throw new KeyNotFoundException($"User with ID {userId} not found.");


		user.UserRoleId = (int)newRole;
		user.UpdatedAt = DateTime.UtcNow.AddHours(2);

		await _context.SaveChangesAsync();

		return new UserDto
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			UserRole = newRole.GetDescription(),
			UpdatedAt = user.UpdatedAt,
		};
	}
}
