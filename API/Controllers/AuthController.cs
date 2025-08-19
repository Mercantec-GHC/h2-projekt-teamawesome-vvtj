using API.Data;
using API.Interfaces;
using API.Services;
using DomainModels.Dto.UserDto;
using DomainModels.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService /*IConfiguration configuration*/, ILoginAttemptService loginAttemptService) : ControllerBase
{
	//The first part of the code(everything commented in) is to test the authentication without a database.
	//For learning purposes, I decided to leave both options with and without sending data to a database.
	//So you have an opportunity to try implementing and test the authentication, login and registration without a database.
	//All the commented code I will delete before the next release Friday.

	//-------------------------------------------------------------------------------------
	//*********************TESTING CODE WITHOUT A DATABASE STARTS HERE*********************
	//--------------------------------------------------------------------------------------
	// Static user instance to test without sending data in a database.

	//public static User user = new()
	//{
	//	Email = string.Empty,
	//	UserName = string.Empty,
	//	HashedPassword = string.Empty,
	//	Salt = string.Empty
	//};

	//[HttpPost("register")]
	//public async Task<ActionResult<RegisterDto>> Register(RegisterDto request)
	//{
	//	var hashedPassword = new PasswordHasher<User>()
	//		.HashPassword(user, request.Password);

	//	user.UserName = request.Username;
	//	user.Email = request.Email;
	//	user.HashedPassword = hashedPassword;
	//	user.Salt = string.Empty;

	//	return Ok(user);
	//}
	//[HttpPost("login")]
	//public ActionResult<string> Login(LoginDto request)
	//{
	//	//For educational purposes, this implementation is not secure for real life applications.
	//	if (user.Email != request.Email)
	//	{
	//		return BadRequest("User not found.");
	//	}
	//	if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.Password)
	//	== PasswordVerificationResult.Failed)
	//	{
	//		return BadRequest("Invalid password.");
	//	}

	//	string token = CreateToken(user);
	//	return Ok(token);
	//}

	//private string CreateToken(User user)
	//{
	//	var claims = new List<Claim>
	//	{
	//		new Claim(ClaimTypes.Name, user.UserName),
	//		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
	//		new Claim(ClaimTypes.Role, user.UserRole.RoleName.ToString())
	//	};
	//	var key = new SymmetricSecurityKey(
	//		Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

	//	var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
	//	var tokenDescriptor = new JwtSecurityToken(
	//		issuer: configuration.GetValue<string>("AppSettings:Issuer"),
	//		audience: configuration.GetValue<string>("AppSettings:Audience"),
	//		claims: claims,
	//		expires: DateTime.Now.AddMinutes(60),
	//		signingCredentials: creds
	//		);

	//	return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
	//}
	//-------------------------------------------------------------------------------------
	//*********************TESTING CODE WITHOUT A DATABASE ENDS HERE*********************
	//--------------------------------------------------------------------------------------

	private readonly AppDBContext _context;

	[HttpPost("register")]
	public async Task<ActionResult<RegisterDto>> Register(RegisterDto request)
	{
		var user = await authService.RegisterUserAsync(request);
		if (user == null)
		{
			return BadRequest("User already exists.");
		}

		return Ok(user);
	}

	[HttpPost("login")]
	public async Task<ActionResult<string>> Login(LoginDto request)
	{
		try
		{
			if (loginAttemptService.IsLockedOut(request.Email))
			{
				var remainingSeconds = loginAttemptService.GetRemainingLockoutSeconds(request.Email);
				return StatusCode(429, new
				{
					message = "Account temporarily locked due to too many failed login attempts.",
					remainingLockoutSeconds = remainingSeconds
				});
			}

			var token = await authService.LoginUserAsync(request);
			if (token == null)
			{
				var attemptsLeft = loginAttemptService.RecordFailedAttempt(request.Email);

				return Unauthorized(new
				{
					message = "Invalid email or password.",
					attempts_left = attemptsLeft + 1
				});
			}

			loginAttemptService.RecordSuccessfulLogin(request.Email);
			return Ok(token);
		}

		catch (Exception ex)
		{
			//_logger.LogError(ex, "Fejl ved hentning af alle brugere");
			return StatusCode(500, "An internal server error occurred while retrieving users.");
        }

		//Can't test it without a database...
	}

	[Authorize(Roles = "Admin")]
	[HttpGet("/me")]
	public IActionResult GetCurrentUser()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userId == null)
			return Unauthorized("Bruger-ID ikke fundet i token.");

		// 2. Slå brugeren op i databasen
		var user = _context.Users
			.FirstOrDefault(u => u.Id.ToString() == userId);

		if (user == null)
			return NotFound("Brugeren blev ikke fundet i databasen.");

		// 3. Returnér ønskede data - fx til profilsiden
		return Ok(new
		{
			Id = user.Id,
			Email = user.Email,
			CreatedAt = user.CreatedAt
		});
	}
}
