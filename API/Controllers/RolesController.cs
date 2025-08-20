using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
	private readonly IRoleService _roleService;

	public RolesController(IRoleService roleService)
	{
		_roleService = roleService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<RoleDto>>> GetRolesAsync()
	{
		var roles = await _roleService.GetAllRolesAsync();
		if (roles == null || !roles.Any())
		{
			return NotFound();
		}
		return Ok(roles);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<RoleDto>> GetRoleByIdAsync(int id)
	{
		var role = await _roleService.GetRoleByIdAsync(id);
		if (role == null)
		{
			return NotFound();
		}
		return Ok(role);
	}
}
//We are going to use Roles as enums. 
//We are not going to have a Post method for Role because we are not going to create roles through the API.
//We are not going to implement the Delete method for Role because we are not going to delete roles through the API for safety reasons.