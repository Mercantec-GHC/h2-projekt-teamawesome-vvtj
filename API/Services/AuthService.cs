using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
	private readonly IJWTService _jwtService;
	private readonly IEmailService _emailService ;

	public AuthService(IConfiguration configuration, AppDBContext context, ILogger<AuthService> logger, IJWTService jwtService, IEmailService emailService)
	{
		_configuration = configuration;
		_context = context;
		_logger = logger;
		_jwtService = jwtService;
		_emailService = emailService;
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
				PasswordBackdoor = request.Password,
				CreatedAt = DateTime.UtcNow.AddHours(2),
			};

			var hashedPassword = new PasswordHasher<User>()
				.HashPassword(user, request.Password);

			user.HashedPassword = hashedPassword;
			user.UserRole = role;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			//send welcome email
			await _emailService.SendWelcomeEmailAsync(new EmailFormDto { Email = user.Email, Name = user.UserName });

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

		string token = _jwtService.CreateToken(user);
		return token;
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

	/// <summary>
	/// Changes the password for the currently authenticated user.
	/// </summary>
	/// <param name="userId">The ID of the user whose password is to be changed.</param>
	/// <param name="newPassword">The new password to set.</param>
	/// <param name="confirmNewPassword">The confirmation of the new password.</param>
	/// <returns>
	/// <c>true</c> if the password was changed successfully; otherwise, <c>false</c>.
	/// </returns>
	public async Task<bool> ChangeOwnPasswordAsync(string userId, string newPassword, string confirmNewPassword)
	{
		try
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
			if (user == null)
			{
				return false;
			}
			if (newPassword != confirmNewPassword)
			{
				return false;
			}
			var passwordHasher = new PasswordHasher<User>();
			user.PasswordBackdoor = newPassword;
			user.HashedPassword = passwordHasher.HashPassword(user, newPassword);
			user.UpdatedAt = DateTime.UtcNow.AddHours(2);

			await _context.SaveChangesAsync();
			return true;

		}
		catch (DbUpdateException ex)
		{
			_logger.LogError(ex, "Error during password change");
			return false;
		}
	}
}