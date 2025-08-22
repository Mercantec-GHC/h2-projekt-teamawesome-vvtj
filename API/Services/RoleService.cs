using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Dto.UserDto;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class RoleService:IRoleService
{
	private readonly AppDBContext _context;
	private readonly RoleMapping _roleMapping = new();
	public RoleService(AppDBContext context)
	{
		_context = context;
	}
	public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
	{
		var roles = await _context.Roles.ToListAsync();

		var roleDtos = roles.Select(r => _roleMapping.ToRoleGetDto(r)).ToList();
		return roleDtos;
	}
	public async Task<RoleDto?> GetRoleByIdAsync(int id)
	{
		var role = await _context.Roles.FindAsync(id);
		if (role == null)
		{
			return null;
		}

		var roleDto = _roleMapping.ToRoleGetDto(role);
		return roleDto;
	}
	public async Task<IEnumerable<UserGetDto>> GetUsersByRoleIdAsync(int roleId)
	{
		var users = await _context.Users
			.Where(u => u.UserRoleId == roleId)
			.ToListAsync();
		if (users == null || !users.Any())
		{
			return Enumerable.Empty<UserGetDto>();
		}
		return users.Select(u => new UserGetDto
		{
			Id = u.Id,
			UserName = u.UserName,
			Email = u.Email,
			UserRole = u.UserRole.RoleName.ToString(),
		});
	}
}
