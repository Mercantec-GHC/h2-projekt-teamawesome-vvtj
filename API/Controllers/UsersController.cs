using System.Security.Claims;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
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
	/// <remarks>
	/// Requires authentication and Admin role.
	/// </remarks>
	[Authorize(Roles = "Admin")]
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
	/// Requires authentication and Admin role.
	/// </remarks>
	[Authorize(Roles = "Admin")]
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
	/// Retrieves the currently authenticated user's profile information.
	/// </summary>
	/// <returns>
	/// An object containing user ID, email, username, creation date, last login, and role if found;
	/// <see cref="UnauthorizedObjectResult"/> if the user ID is not found in the token;
	/// <see cref="NotFoundObjectResult"/> if the user does not exist in the database.
	/// </returns>
	[Authorize]
	[HttpGet("me")]
	public IActionResult GetCurrentUser()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userId == null)
			return Unauthorized("User-ID was not found in a token.");

		var user = _context.Users
			.Include(u => u.UserRole)
			.FirstOrDefault(u => u.Id.ToString() == userId);

		if (user == null)
			return NotFound("User was not found in a database.");

		var roleEnum = (RoleEnum)user.UserRoleId;

		return Ok(new
		{
			Id = user.Id,
			Email = user.Email,
			Username = user.UserName,
			CreatedAt = user.CreatedAt,
			LastLogin = user.LastLogin,
			Role = user.UserRole.RoleName.ToString(),
			Description = roleEnum.GetDescription()
		});
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
}