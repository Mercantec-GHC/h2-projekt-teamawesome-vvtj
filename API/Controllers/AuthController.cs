using System.Security.Claims;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.AuthDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Provides endpoints for user authentication, registration, token management, and password changes.
/// Some endpoints require authentication or specific roles as noted in their documentation.
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
	/// <param name="authService">Service for authentication and user management logic.</param>
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
	/// 
	/// <para><b>Authorization:</b> Not required. Public endpoint.</para>
	/// </summary>
	/// <param name="request">The registration details including email, username, password, and role.</param>
	/// <returns>
	/// <see cref="OkObjectResult"/> with registered user data if successful;
	/// <see cref="BadRequestObjectResult"/> if the user already exists.
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
	/// Authenticates a user and returns JWT access and refresh tokens if credentials are valid.
	/// Handles lockout for repeated failed attempts and logs authentication events.
	/// 
	/// <para><b>Authorization:</b> Not required. Public endpoint.</para>
	/// </summary>
	/// <param name="request">The login details including email and password.</param>
	/// <returns>
	/// <see cref="OkObjectResult"/> with JWT tokens if login is successful;
	/// <see cref="UnauthorizedObjectResult"/> if credentials are invalid;
	/// <see cref="ObjectResult"/> with status code 429 if account is locked out;
	/// <see cref="ObjectResult"/> with status code 500 if an internal error occurs.
	/// </returns>
	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDto request)
	{
		try
		{
			if (_loginAttemptService.IsLockedOut(request.Username))
			{
				var remainingSeconds = _loginAttemptService.GetRemainingLockoutSeconds(request.Username);
				return StatusCode(429, new
				{
					message = "Account temporarily locked due to too many failed login attempts.",
					remainingLockoutSeconds = remainingSeconds
				});
			}

			var result = await _authService.LoginUserAsync(request);
			if (result == null)
			{
				var attemptsLeft = _loginAttemptService.RecordFailedAttempt(request.Username);

				return Unauthorized(new
				{
					message = "Invalid input.",
					attempts_left = attemptsLeft + 1
				});
			}

			_loginAttemptService.RecordSuccessfulLogin(request.Username);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = DateTime.UtcNow.AddDays(7)
			};
			Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

			return Ok(new
			{
				accessToken = result.AccessToken,
				refreshToken = result.RefreshToken
			});
		}

		catch (Exception ex)
		{
			_logger.LogError(ex, "Login failed for email {Username}", request.Username);
			return StatusCode(500, "An internal error occurred. Please try again later.");
		}
	}

	/// <summary>
	/// Refreshes the JWT access token using a valid refresh token.
	/// Validates the refresh token from the request and the cookie, then issues new tokens if valid.
	/// 
	/// <para><b>Authorization:</b> Not required. Public endpoint, but requires a valid refresh token cookie.</para>
	/// </summary>
	/// <returns>
	/// <see cref="OkObjectResult"/> with new access and refresh tokens if successful;
	/// <see cref="BadRequestObjectResult"/> if the refresh token is missing or does not match the cookie;
	/// <see cref="UnauthorizedObjectResult"/> if the refresh token is invalid;
	/// <see cref="ObjectResult"/> with status code 500 if an internal error occurs.
	/// </returns>
	[HttpPost("refresh-token")]
	public async Task<ActionResult<TokenResponseDto>> RefreshToken()
	{
		try
		{
			var refreshToken = Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(refreshToken))
				return BadRequest("Refresh token is missing or does not match.");

			// Use IP/device info for token rotation tracking
			var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
			var device = Request.Headers["User-Agent"].ToString();

			var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress, device);

			if (result == null)
				return Unauthorized("Invalid or expired refresh token.");

			// Overwrite cookie with new refresh token
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = DateTime.UtcNow.AddDays(7)
			};
			Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

			return Ok(new TokenResponseDto
			{
				AccessToken = result.AccessToken
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error refreshing token");
			return StatusCode(500, "An internal error occurred. Please try again later.");
		}
	}

	/// <summary>
	/// Changes the password for a specified user.
	/// 
	/// <para><b>Authorization:</b> Required. Only users with the <c>Admin</c> role can access this endpoint.</para>
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
	/// Allows the currently authenticated user to change their own password.
	/// Validates input and ensures the new password and confirmation match.
	/// 
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <param name="changePassword">The new password and confirmation details.</param>
	/// <returns>
	/// <see cref="OkObjectResult"/> if the password was changed successfully;
	/// <see cref="BadRequestObjectResult"/> if input is invalid or password change fails;
	/// <see cref="UnauthorizedObjectResult"/> if the user is not authenticated;
	/// <see cref="ObjectResult"/> with status code 500 if an error occurs.
	/// </returns>
	[Authorize]
	[HttpPost("change-own-password")]
	public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangePasswordDto changePassword)
	{
		try
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
				return Unauthorized("User ID claim not found.");

			if (string.IsNullOrWhiteSpace(changePassword.NewPassword)
				|| string.IsNullOrWhiteSpace(changePassword.ConfirmPassword))
				return BadRequest("New password and confirmation cannot be empty.");

			var result = await _authService.ChangeOwnPasswordAsync(userId, changePassword.NewPassword, changePassword.ConfirmPassword);

			if (!result)
				return BadRequest("Failed to change password. Please ensure your current password is correct.");
			return Ok("Password changed successfully.");
		}
		catch
		{
			return StatusCode(500, "An error occurred while changing the password.");
		}
	}
}
