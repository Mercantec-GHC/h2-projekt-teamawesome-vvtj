using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainModels.Dto.UserDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
	// Static user instance to test without sending data in a database.
	public static User user = new()
	{
		Email = string.Empty,
		UserName = string.Empty,
		HashedPassword = string.Empty,
		Salt = string.Empty
	}; 

	[HttpPost("register")]
	public async Task<ActionResult<RegisterDto>> Register(RegisterDto request)
	{
		var hashedPassword = new PasswordHasher<User>()
			.HashPassword(user, request.Password);

		user.UserName = request.Username;
		user.Email = request.Email;
		user.HashedPassword = hashedPassword;
		user.Salt = string.Empty;

		return Ok(user);
	}
	[HttpPost("login")]
	public ActionResult<string> Login(LoginDto request)
	{
		//For educational purposes, this implementation is not secure for real life applications.
		if (user.Email != request.Email)
		{
			return BadRequest("User not found.");
		}
		if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.Password)
		== PasswordVerificationResult.Failed)
		{
			return BadRequest("Invalid password.");
		}

		string token = CreateToken(user);
		return token;
	}

	private string CreateToken(User user)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.UserName)
		};
		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
		var tokenDescriptor = new JwtSecurityToken(
			issuer: configuration.GetValue<string>("AppSettings:Issuer"),
			audience: configuration.GetValue<string>("AppSettings:Audience"),
			claims: claims,
			expires: DateTime.Now.AddMinutes(60),
			signingCredentials: creds
			);

		return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
	}
}
