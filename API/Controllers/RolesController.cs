using System.Security.Claims;
using API.Interfaces;
using DomainModels.Dto.RoleDto;
using DomainModels.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
	private readonly IRoleService _roleService;
	private readonly ILogger<RolesController> _logger;

	public RolesController(IRoleService roleService, ILogger<RolesController> logger)
	{
		_roleService = roleService;
		_logger = logger;
	}

	[Authorize(Roles = "Admin")]
	[HttpGet]
	public async Task<ActionResult<IEnumerable<RoleDetailsDto>>> GetRolesAsync()
	{
		var roles = await _roleService.GetAllRolesAsync();
		if (roles == null || !roles.Any())
		{
			return NotFound();
		}
		return Ok(roles);
	}

	[Authorize(Roles = "Admin")]
	[HttpGet("{id:int}")]
	public async Task<ActionResult<RoleDetailsDto>> GetRoleByIdAsync(int id)
	{
		var role = await _roleService.GetRoleByIdAsync(id);
		if (role == null)
		{
			return NotFound();
		}
		return Ok(role);
	}

	[Authorize(Roles = "Admin")]
	[HttpPut("{id:int}/assign-role-to-user")]
	public async Task<ActionResult> AssignUserRole(int id, RoleEnum newRole)
	{
		try
		{
			_logger.LogInformation("Assigned role {RoleId} to user {UserId}", newRole, id);
			var updatedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "System";
			await _roleService.AssignRoleToUserAsync(id, newRole);

			return Ok(new { message = "Role succesfully assigned to user", id, role = newRole.ToString(), updatedBy });
		}

		catch (Exception ex)
		{
			_logger.LogError(ex, "An error while assigning role to the user {UserId}", id);
			return StatusCode(500, "An internal server error occurred while assigning role.");
		}
	}
}