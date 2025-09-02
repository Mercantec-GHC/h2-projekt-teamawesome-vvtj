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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

/// <summary>
/// Provides authentication and registration services for users, including JWT token generation and password management.
/// </summary>
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
	/// <returns>
	/// A <see cref="UserDto"/> containing user details if registration is successful; otherwise, <c>null</c>.
	/// </returns>
	public async Task<UserDto?> RegisterUserAsync(RegisterDto request)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(request.Email)
				|| string.IsNullOrWhiteSpace(request.Username)
				|| string.IsNullOrWhiteSpace(request.Password))
			{
				_logger.LogWarning("Registration failed: Email, Username, or Password is empty.");
				return null;
			}

			if (await _context.Users.AnyAsync(u => u.Email == request.Email))
			{
				_logger.LogWarning("Registration failed: User with email {Email} already exists.", request.Email);
				return null; // User already exists
			}

			var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == RoleEnum.Guest);

		var user = new User
		{
			Email = request.Email,
			UserName = request.Username,
			HashedPassword = string.Empty,
			CreatedAt = DateTime.UtcNow.AddHours(2),
		};

			var hashedPassword = new PasswordHasher<User>()
				.HashPassword(user, request.Password);

		user.HashedPassword = hashedPassword;
		user.UserRole = role;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Registered new user with email: {Email}", request.Email);

			return _userMapping.ToUserDto(user);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during user registration");
			return null; 
		}
	}

	/// <summary>
	/// Authenticates a user and returns a JWT token if successful.
	/// </summary>
	/// <param name="request">Login credentials containing email and password.</param>
	/// <returns>
	/// A JWT token string if login is successful; otherwise, <c>null</c>.
	/// </returns>
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

		string secretKey = _configuration["AppSettings:Token"]!;
		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(secretKey));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Issuer = _configuration.GetValue<string>("AppSettings:Issuer"),
			Audience = _configuration.GetValue<string>("AppSettings:Audience"),
			Expires = DateTime.UtcNow.AddHours(1), 
			SigningCredentials = creds
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	/// <summary>
	/// Changes the password for a user.
	/// </summary>
	/// <param name="userEmail">The email of the user whose password is to be changed.</param>
	/// <param name="newPassword">The new password to set.</param>
	/// <returns>
	/// <c>true</c> if the password was changed successfully; otherwise, <c>false</c>.
	/// </returns>
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