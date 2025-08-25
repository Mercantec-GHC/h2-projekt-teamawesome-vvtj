using System.Security.Claims;
using API.Interfaces;
using DomainModels.Dto.RoleDto;
using DomainModels.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Provides API endpoints for managing roles and assigning roles to users.
/// </summary>
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

	/// <summary>
	/// Retrieves all roles.
	/// </summary>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing a list of <see cref="RoleDetailsDto"/> objects if roles exist;
	/// otherwise, a 404 Not Found response.
	/// </returns>
	/// <remarks>
	/// Requires authentication and Admin role.
	/// </remarks>
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

	/// <summary>
	/// Retrieves a role by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the role.</param>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing the <see cref="RoleDetailsDto"/> if found;
	/// otherwise, a 404 Not Found response.
	/// </returns>
	/// <remarks>
	/// Requires authentication and Admin role.
	/// </remarks>
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

	/// <summary>
	/// Assigns a new role to a user.
	/// </summary>
	/// <param name="id">The unique identifier of the user to update.</param>
	/// <param name="newRole">The new role to assign to the user.</param>
	/// <returns>
	/// An <see cref="ActionResult"/> indicating the result of the operation:
	/// <list type="bullet">
	/// <item><description>200 OK if the role was successfully assigned.</description></item>
	/// <item><description>500 Internal Server Error if an exception occurs.</description></item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Requires authentication and Admin role.
	/// </remarks>
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