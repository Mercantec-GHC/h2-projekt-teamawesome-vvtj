using System.Security.Cryptography;
using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Dto.AuthDto;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
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

		// Only query tokens that are not revoked and not expired
		var now = DateTime.UtcNow.AddHours(2);
		var existingTokens = await _context.RefreshTokens
			.Where(rt => rt.UserId == user.Id && rt.Expires > now && rt.Revoked == null)
			.ToListAsync();

		if (existingTokens.Count > 0)
		{
			foreach (var token in existingTokens)
			{
				token.Revoked = now;
				token.ReplacedByToken = refreshToken.Token;
			}
		}

		// Avoid unnecessary allocation
		if (user.RefreshTokens == null)
			user.RefreshTokens = new List<RefreshToken>(1);
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

	public async Task<TokenResponseDto?> RefreshTokenAsync(string token, string ipAddress, string device)
	{
		// Find the existing refresh token
		var existingToken = await _context.RefreshTokens
			.Include(rt => rt.User)
			.ThenInclude(rt => rt.UserRole)
			.FirstOrDefaultAsync(rt => rt.Token == token);

		// If invalid, expired, or revoked, reject
		if (existingToken == null || existingToken.Expires <= DateTime.UtcNow.AddHours(2)
			|| existingToken.Revoked != null)
			return null;

		// Revoke the old token
		existingToken.Revoked = DateTime.UtcNow.AddHours(2);

		// Create a new refresh token and save
		var newRefreshToken = await CreateRefreshTokenAsync(ipAddress, device);
		newRefreshToken.UserId = existingToken.UserId;

		_context.RefreshTokens.Add(newRefreshToken);

		// Generate a new JWT access token
		var accessToken = _jwtService.CreateToken(existingToken.User);
		await _context.SaveChangesAsync();

		return new TokenResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = newRefreshToken.Token
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