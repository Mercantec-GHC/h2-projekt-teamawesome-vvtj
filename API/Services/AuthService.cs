using System.Linq;
using System.Security.Cryptography;
using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Dto.AuthDto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AuthService : IAuthService
{
	private readonly AppDBContext _context;
	private readonly UserMapping _userMapping = new();
	private readonly ILogger<AuthService> _logger;
	private readonly IJWTService _jwtService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IEmailService _emailService;

	public AuthService(AppDBContext context, ILogger<AuthService> logger, IJWTService jwtService, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
	{
		_context = context;
		_logger = logger;
		_jwtService = jwtService;
		_httpContextAccessor = httpContextAccessor;
		_emailService = emailService;
	}

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

	public async Task<TokenResponseDto?> LoginUserAsync(LoginDto request)
	{
		var httpContext = _httpContextAccessor.HttpContext;
		var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
		var device = httpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;

		var normalizedEmail = request.Email.ToLowerInvariant();

		var user = await _context.Users
		.Include(u => u.UserRole)
		.Include(u => u.RefreshTokens)
		.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

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

		string accessToken = _jwtService.CreateToken(user);
		var refreshToken = await CreateRefreshTokenAsync(ipAddress, device);

		// Finds all valid (not expired, not revoked) refresh tokens for this user.
		var existingTokens = await _context.RefreshTokens
		.Where(rt => rt.UserId == user.Id && rt.Expires > DateTime.UtcNow.AddHours(2) && rt.Revoked == null)
		.ToListAsync();

		// Revokes all existing valid tokens, linking them to the new token.
		// This prevents reuse of multiple active refresh tokens, reducing risk if a token is leaked.
		foreach (var token in existingTokens)
		{
			token.Revoked = DateTime.UtcNow.AddHours(2);
			token.ReplacedByToken = refreshToken.Token;
		}
		// Ensures the user’s refresh token collection is initialized.
		// Adds the new refresh token to the user and saves all changes in the database.
		user.RefreshTokens ??= new List<RefreshToken>();
		user.RefreshTokens.Add(refreshToken);
		await _context.SaveChangesAsync();

		return new TokenResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken.Token,
		};
	}

	public async Task<RefreshToken?> CreateRefreshTokenAsync(string ipAddress, string device)
	{
		return new RefreshToken
		{
			Token = GenerateRefreshToken(),
			CreatedByIp = ipAddress,
			Device = device,
			Created = DateTime.UtcNow.AddHours(2),
			Expires = DateTime.UtcNow.AddDays(7)
		};
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	public async Task<TokenResponseDto?> RefreshTokenAsync(string token)
	{
		var existingToken = await _context.RefreshTokens
			.Include(rt => rt.User)
			.ThenInclude(rt => rt.UserRole)
			.FirstOrDefaultAsync(rt => rt.Token == token);

		if (existingToken == null || existingToken.Expires <= DateTime.UtcNow.AddHours(2)
			|| existingToken.Revoked != null)
			return null;

		existingToken.Revoked = DateTime.UtcNow.AddHours(2);

		var accessToken = _jwtService.CreateToken(existingToken.User);

		await _context.SaveChangesAsync();

		return new TokenResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = existingToken.Token
		};
	}

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