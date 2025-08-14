using API.Interfaces;
using API.Services;
using DomainModels.Dto.RoleDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
	private readonly IRoleService _roleService;

	public RolesController(IRoleService roleService)
	{
		roleService = _roleService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<RoleGetDto>>> GetRolesAsync()
	{
		var roles = await _roleService.GetAllRolesAsync();
		if (roles == null || !roles.Any())
		{
			return NotFound();
		}
		return Ok(roles);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<RoleGetDto>> GetRoleByIdAsync(int id)
	{
		var role = await _roleService.GetRoleByIdAsync(id);
		if (role == null)
		{
			return NotFound();
		}
		return Ok(role);
	}
	[HttpPost]
	public async Task<IActionResult> CreateRoleAsync([FromBody] RolePostDto roleDto)
	{
		if (roleDto == null)
		{
			return BadRequest("Role's data is null.");
		}
		var createdRole = await _roleService.CreateRoleAsync(roleDto);
		if (createdRole == null)
		{
			return BadRequest("Failed to create role.");
		}
		return CreatedAtAction(nameof(GetRoleByIdAsync), new { id = createdRole.Id }, createdRole);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteRole(int id)
	{
		if (id <= 0)
		{
			return BadRequest("Need a valid role id.");
		}
		var deleted = await _roleService.DeleteRoleAsync(id);
		if (!deleted)
		{
			return NotFound();
		}
		return NoContent();
	}
}