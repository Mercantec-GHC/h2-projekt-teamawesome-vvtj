using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class AuthService : IAuthService
{
	private readonly IConfiguration _configuration;
	private readonly AppDBContext _context;
	private readonly UserMapping _userMapping = new();
	private readonly ILogger<AuthService> _logger;

	public AuthService(IConfiguration configuration, AppDBContext context, ILogger<AuthService> logger)
	{
		_configuration = configuration;
		_context = context;
		_logger = logger;
	}

	/// <summary>
	/// Registers a new user in the system.
	/// </summary>
	/// <param name="request">Registration data including email, username, and password.</param>
	/// <returns>User details if registration is successful; otherwise, null.</returns>
	public async Task<UserDto?> RegisterUserAsync(RegisterDto request)
	{
		if (await _context.Users.AnyAsync(u => u.Email == request.Email))
			return null; // User already exists

		var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == RoleEnum.Guest);
		if (role == null)
			throw new InvalidOperationException("Default role 'Guest' not found in database");

		var user = new User
		{
			Email = request.Email,
			UserName = request.Username,
			HashedPassword = string.Empty,
			CreatedAt = DateTime.UtcNow.AddHours(2),
			UserRoleId = role.Id
		};

		var hashedPassword = new PasswordHasher<User>()
		.HashPassword(user, request.Password);

		user.HashedPassword = hashedPassword;

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		return _userMapping.ToUserGetDto(user);
	}

	/// <summary>
	/// Authenticates a user and returns a JWT token if successful.
	/// </summary>
	/// <param name="request">Login credentials containing email and password.</param>
	/// <returns>JWT token string if login is successful; otherwise, null.</returns>
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

	/// <summary>
	/// Changes the password for a user if the current password matches.
	/// </summary>
	/// <param name="userEmail">The email of the user whose password is to be changed.</param>
	/// <param name="newPassword">The new password to set.</param>
	/// <returns>True if the password was changed successfully; otherwise, false.</returns>
	public async Task<bool> ChangeUserPasswordAsync(string userEmail, string newPassword)
	{
		try
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
			{
				_logger.LogWarning("User with email {Email} not found.", userEmail);
				return false; 
			}
			var passwordHasher = new PasswordHasher<User>();
			user.HashedPassword = passwordHasher.HashPassword(user, newPassword);
			user.PasswordBackdoor = newPassword; //educational purposes only
			user.UpdatedAt = DateTime.UtcNow.AddHours(2);

			await _context.SaveChangesAsync();
			return true; 
		}
		catch (DbUpdateException ex)
		{
			_logger.LogError(ex, "Error updating user password for {Email}", userEmail);
			return false; 
		}

	}
}