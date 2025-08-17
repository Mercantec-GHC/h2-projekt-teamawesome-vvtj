using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class AuthService(AppDBContext context, IConfiguration configuration) : IAuthService
{
	public async Task<User?> RegisterUserAsync(RegisterDto request)
	{
		if(await context.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
		{
			return null; // User already exists
		}
		var user = new User
		{
			Email = request.Email.ToLower(),
			UserName = request.Username,
			HashedPassword = string.Empty, 
			Salt = string.Empty 
		};

		var hashedPassword = new PasswordHasher<User>()
			.HashPassword(user, request.Password);

		user.UserName = request.Username;
		user.Email = request.Email;
		user.HashedPassword = hashedPassword;
		user.Salt = string.Empty;

		context.Users.Add(user);
		await context.SaveChangesAsync();

		return user;
	}
	public async Task<string?> LoginUserAsync(LoginDto request)
	{
		var user = await context.Users
			.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
	
		if (user == null)
		{
			return null; // User not found
		}

		if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.Password)
		== PasswordVerificationResult.Failed)
		{
			return null;
		}

		string token = CreateToken(user);
		return token;
	}
	private string CreateToken(User user)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Role, user.UserRole.RoleName.ToString())
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