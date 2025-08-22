using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Provides API endpoints for managing user accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly ILogger<UsersController> _logger;
	private readonly AppDBContext _context;

	/// <summary>
	/// Initializes a new instance of the <see cref="UsersController"/> class.
	/// </summary>
	/// <param name="userService">The user service for user operations.</param>
	/// <param name="logger">The logger instance for logging errors and information.</param>
	public UsersController(IUserService userService, ILogger<UsersController> logger, AppDBContext context)
	{
		_userService = userService;
		_logger = logger;
		_context = context;
	}

	/// <summary>
	/// Retrieves all users.
	/// </summary>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing a list of <see cref="UserDto"/> objects if users exist;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
	{
		try
		{
			var users = await _userService.GetAllUsersAsync();
			if (users == null || !users.Any())
			{
				return NotFound("No users found.");
			}
			return Ok(users);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while retrieving users.");
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	/// <summary>
	/// Retrieves a user by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing the <see cref="UserDto"/> if found;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	/// <remarks>
	/// Requires authentication.
	/// </remarks>
	[Authorize]
	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserDto>> GetUserById(int id)
	{
		try
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while retrieving user with ID {Id}.", id);
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	/// <summary>
	/// Deletes a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user to delete.</param>
	/// <returns>
	/// A 204 No Content response if successful; 400 Bad Request if email is missing; 404 Not Found if user does not exist;
	/// or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	/// <remarks>
	/// Requires authentication and Admin role.
	/// </remarks>
	[Authorize(Roles = "Admin")]
	[HttpDelete("{email}")]
	public async Task<IActionResult> DeleteUserByEmail(string email)
	{
		try
		{
			if (string.IsNullOrEmpty(email))
			{
				return BadRequest("Email is required.");
			}
			var deleted = await _userService.DeleteUserByEmailAsync(email);
			if (!deleted)
			{
				return NotFound($"User with email '{email}' not found.");
			}
			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while deleting user with email {Email}.", email);
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	/// <summary>
	/// Assigns a new role to a user by updating the user's role in the database.
	/// </summary>
	/// <param name="id">The unique identifier of the user to update.</param>
	/// <param name="dto">The data transfer object containing the role ID to assign.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> indicating the result of the operation:
	/// <list type="bullet">
	/// <item><description>200 OK if the role was successfully assigned.</description></item>
	/// <item><description>404 Not Found if the user does not exist.</description></item>
	/// <item><description>400 Bad Request if the role is invalid.</description></item>
	/// <item><description>500 Internal Server Error if an exception occurs.</description></item>
	/// </list>
	/// </returns>
	[HttpPut("{id}/role")]
	public async Task<ActionResult> AssignUserRole(int id, AssignRoleDto dto)
	{
		try
		{
			_logger.LogInformation("Assigned role {RoleId} to user {UserId}", dto.RoleId, id);

			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				_logger.LogWarning("User with ID {UserId} was not found", id);
				return NotFound("User not found.");
			}

			var role = await _context.Roles.FindAsync(dto.RoleId);
			if (role == null)
			{
				_logger.LogWarning("Role with ID {RoleId} was not found", dto.RoleId);
				return BadRequest("Invalid role.");
			}

			user.UserRoleId = dto.RoleId;
			user.UpdatedAt = DateTime.UtcNow.AddHours(2);

			await _context.SaveChangesAsync();

			_logger.LogInformation("Role {RoleName} was assigned to user {UserEmail}", role.RoleName, user.Email);

			return Ok(new { message = "Role succesfully assigned to user", user.Email, role = role.RoleName });
		}
		
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error while assigning role to the user {UserId}", id);
			return StatusCode(500, "An internal server error occurred while assigning role.");
		}
	}
}