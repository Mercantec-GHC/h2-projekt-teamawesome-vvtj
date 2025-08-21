using API.Interfaces;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

	/// <summary>
	/// Initializes a new instance of the <see cref="UsersController"/> class.
	/// </summary>
	/// <param name="userService">The user service for user operations.</param>
	/// <param name="logger">The logger instance for logging errors and information.</param>
	public UsersController(IUserService userService, ILogger<UsersController> logger)
	{
		_userService = userService;
		_logger = logger;
	}

	/// <summary>
	/// Retrieves all users.
	/// </summary>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing a list of <see cref="UserGetDto"/> objects if users exist;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserGetDto>>> GetUsers()
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
	/// An <see cref="ActionResult{T}"/> containing the <see cref="UserGetDto"/> if found;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	/// <remarks>
	/// Requires authentication.
	/// </remarks>
	[Authorize]
	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserGetDto>> GetUserById(int id)
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
	/// Updates an existing user's information.
	/// </summary>
	/// <param name="userDto">The user data to update.</param>
	/// <returns>
	/// A 204 No Content response if successful; 400 Bad Request if input is invalid; 404 Not Found if user does not exist;
	/// or a 500 Internal Server Error if an exception occurs.
	/// </returns>
	/// <remarks>
	/// Requires authentication and either Admin or Reception role.
	/// </remarks>
	[Authorize(Roles = "Admin,Reception")]
	[HttpPut]
	public async Task<IActionResult> UpdateUser([FromBody] UserPostDto userDto)
	{
		try
		{
			if (userDto == null)
			{
				return BadRequest("User data is null.");
			}
			var updated = await _userService.UpdateUserAsync(userDto);
			if (!updated)
			{
				return NotFound();
			}
			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while updating user.");
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
}