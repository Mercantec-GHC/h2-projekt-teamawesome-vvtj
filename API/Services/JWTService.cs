using API.Interfaces;
using DomainModels.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static API.Services.ActiveDirectoryService;

namespace API.Services;

public class JWTService : IJWTService
{
	private readonly ILogger<JWTService> _logger;
	private readonly IConfiguration _configuration;
	public JWTService(ILogger<JWTService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

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
                new Claim("adGroups", string.Join(",", adUser.Groups))
		});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating token for AD user {SamAccountName}", adUser.SamAccountName);
			throw;
		}
	}

	public string GenerateToken(IEnumerable<Claim> claims)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var secretKey = _configuration["AppSettings:Token"]!;
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
		var claimsIdentity = new ClaimsIdentity(claims);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = claimsIdentity,
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}
