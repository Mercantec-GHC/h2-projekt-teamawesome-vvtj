using Microsoft.AspNetCore.Mvc;
using API.Services;
using DomainModels.Dto.UserDto;

namespace API.Controllers;
/// <summary>
/// Controller for handling Active Directory authentication and related operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ActiveDirectoryController : ControllerBase
{

	private readonly ActiveDirectoryService _activeDirectoryService;
	private readonly ILogger<ActiveDirectoryController> _logger;
	/// <summary>
	/// Initializes a new instance of the <see cref="ActiveDirectoryController"/> class.
	/// </summary>
	/// <param name="activeDirectoryService">Service for Active Directory operations.</param>

	public ActiveDirectoryController(ActiveDirectoryService activeDirectoryService, ILogger<ActiveDirectoryController> logger)
	{
		_activeDirectoryService = activeDirectoryService;
		_logger = logger;
	}

	///<summary>
	/// Authenticates an Active Directory user and returns a JWT token if successful.
	/// </summary>
	/// <param name="ADdto">DTO containing the AD username and password.</param>
	/// <returns>
	/// 200 OK with a JWT token if authentication succeeds; 401 Unauthorized with error message if it fails.
	/// </returns>
	[HttpPost("login-ad")]
	public async Task<IActionResult> ADLogin(ADLoginDto ADdto)
	{
		try
		{
			var token = await _activeDirectoryService.LoginADUserAsync(ADdto.Username, ADdto.Password);
			if (token == null)
			{
				return Unauthorized(new
				{
					message = "Invalid AD credentials."
				});
			}

			return Ok(new TokenResponseDto
			{
				Token = token
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during AD login for user {Username}", ADdto.Username);
			return StatusCode(500, new
			{
				message = "An error occurred while processing your request."
			});
		}

	}

}
