using System.Security.Claims;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Provides API endpoints for managing user accounts.
/// <para>Some endpoints require authentication and/or specific roles as noted in their documentation.</para>
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
	/// <param name="context">The database context.</param>
	public UsersController(IUserService userService, ILogger<UsersController> logger, AppDBContext context)
	{
		_userService = userService;
		_logger = logger;
		_context = context;
	}

	/// <summary>
	/// Retrieves all users.
	/// <para><b>Authorization:</b> Required. Only users with the <c>Admin</c> role can access this endpoint.</para>
	/// </summary>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing a list of <see cref="UserDto"/> objects if users exist;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
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
	/// <para><b>Authorization:</b> Required. Only users with the <c>Admin</c> role can access this endpoint.</para>
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing the <see cref="UserDto"/> if found;
	/// otherwise, a 404 Not Found response or a 500 Internal Server Error if an exception occurs.
	/// </returns>
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
	/// Retrieves the profile of the currently authenticated user.
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <remarks>
	/// The user's identifier is extracted from the authentication token.
	/// </remarks>
	/// <returns>
	/// An <see cref="ActionResult{T}"/> containing:
	/// <list type="bullet">
	///   <item>
	///     <description><see cref="UserDtoUnsafe"/> if the user is found.</description>
	///   </item>
	///   <item>
	///     <description><see cref="UnauthorizedResult"/> if the user ID claim is missing.</description>
	///   </item>
	///   <item>
	///     <description><see cref="NotFoundResult"/> if the user profile cannot be found.</description>
	///   </item>
	/// </list>
	/// </returns>
	[Authorize]
	[HttpGet("me")]
	public async Task<ActionResult<UserDtoUnsafe>> GetCurrentUser()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userId == null)
			return Unauthorized("User-ID was not found in a token.");

		var user = await _userService.GetUserFromTokenAsync(userId);

		if (user == null)
			return NotFound();

		return Ok(user);

	}

	/// <summary>
	/// Deletes a user by their email address.
	/// <para><b>Authorization:</b> Required. Only users with the <c>Admin</c> role can access this endpoint.</para>
	/// </summary>
	/// <param name="email">The email address of the user to delete.</param>
	/// <returns>
	/// A 204 No Content response if successful; 400 Bad Request if email is missing; 404 Not Found if user does not exist;
	/// or a 500 Internal Server Error if an exception occurs.
	/// </returns>
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