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
/// Provides endpoints for user authentication, registration, profile retrieval and password changing.
/// </summary>

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly ILoginAttemptService _loginAttemptService;
	private readonly ILogger<AuthController> _logger;
	private readonly AppDBContext _context;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthController"/> class.
	/// </summary>
	/// <param name="authService">Service for authentication logic.</param>
	/// <param name="loginAttemptService">Service for tracking login attempts and lockouts.</param>
	/// <param name="logger">Logger for authentication events.</param>
	/// <param name="context">Database context for user data.</param>
	public AuthController(IAuthService authService, ILoginAttemptService loginAttemptService, ILogger<AuthController> logger, AppDBContext context)
	{
		_authService = authService;
		_loginAttemptService = loginAttemptService;
		_logger = logger;
		_context = context;
	}

	/// <summary>
	/// Registers a new user account.
	/// </summary>
	/// <param name="request">The registration details including email, username, password, and role.</param>
	/// <returns>
	/// <see cref="RegisterDto"/> with registered user data if successful;
	/// otherwise, a <see cref="BadRequestObjectResult"/> if the user already exists.
	/// </returns>
	[HttpPost("register")]
	public async Task<ActionResult<RegisterDto>> Register(RegisterDto request)
	{
		var user = await _authService.RegisterUserAsync(request);
		if (user == null)
		{
			return BadRequest("User already exists.");
		}

		return Ok(user);
	}

	/// <summary>
	/// Authenticates a user and returns a JWT token if credentials are valid.
	/// </summary>
	/// <param name="request">The login details including email and password.</param>
	/// <returns>
	/// JWT token as a string if login is successful;
	/// <see cref="UnauthorizedObjectResult"/> if credentials are invalid;
	/// <see cref="ObjectResult"/> with status code 429 if account is locked out;
	/// <see cref="ObjectResult"/> with status code 500 if an internal error occurs.
	/// </returns>
	[HttpPost("login")]
	public async Task<ActionResult<string>> Login(LoginDto request)
	{
		try
		{
			if (_loginAttemptService.IsLockedOut(request.Email))
			{
				var remainingSeconds = _loginAttemptService.GetRemainingLockoutSeconds(request.Email);
				return StatusCode(429, new
				{
					message = "Account temporarily locked due to too many failed login attempts.",
					remainingLockoutSeconds = remainingSeconds
				});
			}

			var token = await _authService.LoginUserAsync(request);
			if (token == null)
			{
				var attemptsLeft = _loginAttemptService.RecordFailedAttempt(request.Email);

				return Unauthorized(new
				{
					message = "Invalid email or password.",
					attempts_left = attemptsLeft + 1
				});
			}

			_loginAttemptService.RecordSuccessfulLogin(request.Email);
			return Ok(token);
		}

		catch (Exception ex)
		{
			_logger.LogError(ex, "Login failed for email {Email}", request.Email);
			return StatusCode(500, "An internal error occurred. Please try again later.");
		}
	}

	/// <summary>
	/// Changes the password for a specified user.
	/// </summary>
	/// <param name="email">The email address of the user whose password is to be changed.</param>
	/// <param name="request">The new password details.</param>
	/// <returns>
	/// <see cref="OkObjectResult"/> if the password was changed successfully;
	/// <see cref="BadRequestObjectResult"/> if the new password is empty;
	/// <see cref="NotFoundObjectResult"/> if the user was not found or the password update failed.
	/// </returns>
	[Authorize(Roles = "Admin")]
	[HttpPost("change-password/{email}")]
	public async Task<IActionResult> ChangePassword(string email, [FromBody] ChangePasswordDto request)
	{
		try
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return NotFound("User not found.");

			if (string.IsNullOrWhiteSpace(request.NewPassword))
				return BadRequest("New password cannot be empty.");

			var result = await _authService.ChangeUserPasswordAsync(email, request.NewPassword);
			
			if (!result)
				return NotFound("User not found or failed to update password.");


			_logger.LogInformation("Password changed successfully for user with email {Email}", email);

			return Ok("Password changed successfully.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error changing password for user with email {Email}", email);
			return StatusCode(500, "An error occurred while retrieving the user.");
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
	[HttpGet("/me")]
	public IActionResult GetCurrentUser()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userId == null)
			return Unauthorized("Bruger-ID ikke fundet i token.");

		var user = _context.Users
			.Include(u => u.UserRole)
			.FirstOrDefault(u => u.Id.ToString() == userId);

		if (user == null)
			return NotFound("Brugeren blev ikke fundet i databasen.");

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
}
