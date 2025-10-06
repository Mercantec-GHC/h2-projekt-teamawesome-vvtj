using API.Data;
using API.Interfaces;
using DomainModels.Dto.RoleDto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

/// <summary>
/// Provides business logic for managing user roles and role assignments in the application.
/// Supports retrieving roles, fetching users by role, and assigning roles to users.
/// </summary>
public class RoleService : IRoleService
{
	private readonly AppDBContext _context;
	private readonly RoleMapping _roleMapping = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="RoleService"/> class.
	/// </summary>
	/// <param name="context">The database context for accessing roles and users.</param>
	public RoleService(AppDBContext context)
	{
		_context = context;
	}

	/// <summary>
	/// Retrieves all roles defined in the system.
	/// </summary>
	/// <returns>
	/// An <see cref="IEnumerable{RoleDetailsDto}"/> containing all available roles.
	/// </returns>
	public async Task<IEnumerable<RoleDetailsDto>> GetAllRolesAsync()
	{
		var roles = await _context.Roles.ToListAsync();

		var roleDtos = roles.Select(r => _roleMapping.ToRoleDto(r)).ToList();
		return roleDtos;
	}

	/// <summary>
	/// Retrieves details for a specific role by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the role.</param>
	/// <returns>
	/// A <see cref="RoleDetailsDto"/> if the role exists; otherwise, <c>null</c>.
	/// </returns>
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

	/// <summary>
	/// Retrieves all users assigned to a specific role.
	/// </summary>
	/// <param name="roleId">The unique identifier of the role.</param>
	/// <returns>
	/// An <see cref="IEnumerable{UserDto}"/> containing users assigned to the specified role.
	/// Returns an empty collection if no users are found.
	/// </returns>
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
	/// Assigns a new role to a user and returns the updated user information.
	/// </summary>
	/// <param name="userId">The unique identifier of the user to update.</param>
	/// <param name="newRole">The new role to assign to the user.</param>
	/// <returns>
	/// A <see cref="UserDto"/> representing the updated user with the new role.
	/// </returns>
	public async Task<UserDto> AssignRoleToUserAsync(int userId, RoleEnum newRole)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
