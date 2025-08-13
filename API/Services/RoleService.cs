using API.Data;
using API.Interfaces;
using DomainModels.Dto.RoleDto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class RoleService:IRoleService
{
	private readonly AppDBContext _context;
	public RoleService(AppDBContext context)
	{
		_context = context;
	}
	public async Task<IEnumerable<RoleGetDto>> GetAllRolesAsync()
	{
		var roles = await _context.Roles.ToListAsync();
		return roles.Select(r => new RoleGetDto
		{
			Id = r.Id,
			RoleName = r.RoleName
		});
	}
	public async Task<RoleGetDto?> GetRoleByIdAsync(int id)
	{
		var role = await _context.Roles.FindAsync(id);
		if (role == null)
		{
			return null;
		}
		return new RoleGetDto
		{
			Id = role.Id,
			RoleName = role.RoleName
		};
	}
	public async Task<RolePostDto?> CreateRoleAsync(RolePostDto roleDto)
	{
		var newRole = new Role
		{
			RoleName = roleDto.RoleName
		};

		await _context.Roles.AddAsync(newRole);
		await _context.SaveChangesAsync();

		return new RolePostDto
		{
			Id = newRole.Id,
			RoleName = newRole.RoleName
		};
	}
	public async Task<bool> DeleteRoleAsync(int id)
	{
		var role = await _context.Roles.FindAsync(id);
		if (role == null)
		{
			return false;
		}
		_context.Roles.Remove(role);
		await _context.SaveChangesAsync();
		return true;
	}
}
