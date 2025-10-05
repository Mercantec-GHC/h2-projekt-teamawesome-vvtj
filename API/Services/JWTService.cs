using API.Interfaces;
using DomainModels.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static API.Services.ActiveDirectoryService;

namespace API.Services;

/// <summary>
/// Provides functionality for generating JWT access tokens for both application and Active Directory users.
/// Used for authentication and session management in Blazor WebAssembly applications.
/// </summary>
public class JWTService : IJWTService
{
	private readonly ILogger<JWTService> _logger;
	private readonly IConfiguration _configuration;

	/// <summary>
	/// Initializes a new instance of the <see cref="JWTService"/> class.
	/// </summary>
	/// <param name="logger">Logger for recording token generation events and errors.</param>
	/// <param name="configuration">Application configuration for retrieving JWT settings.</param>
	public JWTService(ILogger<JWTService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	/// <summary>
	/// Generates a JWT access token for a standard application user.
	/// The token includes user claims and roles for secure authentication.
	/// </summary>
	/// <param name="user">The application user for whom the token is generated.</param>
	/// <returns>
	/// A JWT token string representing the authenticated user.
	/// </returns>
	public string CreateToken(User user)
	{
		try
		{
			return GenerateToken(new List<Claim> {
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, user.UserRole.RoleName.ToString()),
			new Claim(ClaimTypes.Name, user.UserName),

		});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating token for user {UserId}", user.Id);
			throw;
		}
	}

	/// <summary>
	/// Generates a JWT access token for an Active Directory user.
	/// The token includes AD user claims and group memberships for secure authentication.
	/// </summary>
	/// <param name="adUser">The Active Directory user information.</param>
	/// <returns>
	/// A JWT token string representing the authenticated AD user.
	/// </returns>
	public string CreateToken(ADUserInfo adUser)
	{
		try
		{
			return GenerateToken(new List<Claim> {
			new Claim(ClaimTypes.NameIdentifier, adUser.SamAccountName),
				new Claim(ClaimTypes.Email, adUser.Email),
				new Claim("userId", adUser.SamAccountName),
				new Claim("username", adUser.SamAccountName),
				new Claim("adUser", "true"), //Indicate that the user is AD-authenticated
                new Claim("adGroups", string.Join(",", adUser.Groups)),
				new Claim("adDistinguishedName", adUser.DistinguishedName),
				new Claim("role", adUser.Role.ToString()),
				new Claim("department", adUser.Department)
		});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating token for AD user {SamAccountName}", adUser.SamAccountName);
			throw;
		}
	}

	/// <summary>
	/// Generates a JWT token string from the provided claims.
	/// Used internally to create tokens for both application and AD users.
	/// </summary>
	/// <param name="claims">A collection of claims to include in the token.</param>
	/// <returns>
	/// A JWT token string containing the specified claims.
	/// </returns>
	public string GenerateToken(IEnumerable<Claim> claims)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var secretKey = _configuration["AppSettings:Token"]!;
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
		var claimsIdentity = new ClaimsIdentity(claims);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = claimsIdentity,
			Issuer = _configuration.GetValue<string>("AppSettings:Issuer"),
			Audience = _configuration.GetValue<string>("AppSettings:Audience"),
			Expires = DateTime.UtcNow.AddMinutes(5),
			SigningCredentials = creds
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}
