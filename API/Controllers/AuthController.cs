using System.Security.Claims;
using API.Data;
using API.Interfaces;
using API.Services;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Controller for authentication-related actions.
/// </summary>

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly AppDBContext _context;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthController"/> class.
	/// </summary>
	/// <param name="authService">Authentication service.</param>
	/// <param name="context">Database context.</param>
	public AuthController(IAuthService authService, AppDBContext context)
	{
		_authService = authService;
		_context = context;
	}

	/// <summary>
	/// Registers a new user.
	/// </summary>
	/// <param name="request">Registration data.</param>
	/// <returns>Registered user data or error.</returns>
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
	/// Logs in a user.
	/// </summary>
	/// <param name="request">Login data.</param>
	/// <returns>JWT token or error.</returns>
	[HttpPost("login")]
	public async Task<ActionResult<string>> Login(LoginDto request)
	{
		var token = await _authService.LoginUserAsync(request);
		if (token == null)
		{
			return BadRequest("Invalid email or password.");
		}

		return Ok(token);
	}

	/// <summary>
	/// Gets the current authenticated user.
	/// </summary>
	/// <returns>User data or error.</returns>
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

		return Ok(new
		{
			Id = user.Id,
			Email = user.Email,
			Username = user.UserName,
			CreatedAt = user.CreatedAt,
			LastLogin = user.LastLogin,
			Role = user.UserRole.RoleName.ToString(),
		});
	}
}
