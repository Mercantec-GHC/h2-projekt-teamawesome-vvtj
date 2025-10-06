using System.Security.Claims;
using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserProfileDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Provides API endpoints for managing user profile information.
/// <para>All endpoints require authentication unless otherwise specified.</para>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
	private readonly IUserInfoService _userInfoService;
	private readonly AppDBContext _context;

	public UserInfoController(IUserInfoService userInfoService, AppDBContext context)
	{
		_userInfoService = userInfoService;
		_context = context;
	}

	/// <summary>
	/// Retrieves user profile information by user ID.
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> containing the user profile information if found;
	/// otherwise, a 404 Not Found response.
	/// </returns>
	[Authorize]
	[HttpGet("{userId}")]
	public async Task<IActionResult> GetUserInfoByUserId(int userId)
	{
		var userInfo = await _userInfoService.GetByUserIdAsync(userId);
		if (userInfo == null)
		{
			return NotFound();
		}
		return Ok(userInfo);
	}

	/// <summary>
	/// Retrieves the profile information for the currently authenticated user.
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <returns>
	/// An <see cref="ActionResult{UserInfoGetDto}"/> containing the current user's profile information if found;
	/// otherwise, a 404 Not Found or 401 Unauthorized response.
	/// </returns>
	[Authorize]
	[HttpGet("info")]
	public async Task<ActionResult<UserInfoGetDto>> GetCurrentUserInfo()
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId == null)
		{
			return Unauthorized();
		}
		var userInfo = await _userInfoService.GetCurrentUserInfoAsync(userId);
		if (userInfo == null)
		{
			return NotFound();
		}
		return Ok(userInfo);

	}


	//INFO: For educational purposes I have two different implementations:
	//for updating UserInfo I am using service _userInfoService, but for creating UserInfo I am not using services.

	/// <summary>
	/// Updates user profile information for the currently authenticated user.
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <param name="dto">The updated user profile data.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> containing the updated user profile information if successful;
	/// otherwise, a 404 Not Found or 401 Unauthorized response.
	/// </returns>
	[Authorize]
	[HttpPut("update-my-profile")]
	public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfoPutDto dto)
	{
		var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userIdStr == null)
			return Unauthorized("UserId is not authorized.");

		if (!int.TryParse(userIdStr, out var userId))
			return Unauthorized("UserId is not a valid integer.");

		var userInfo = await _context.UserInfos.FirstOrDefaultAsync(ui => ui.UserId == userId);

		if (userInfo == null)
			return NotFound("User was not found in database.");

		var updatedUserInfo = await _userInfoService.UpdateUserInfoAsync(userId, dto);

		if (updatedUserInfo == null)
		{
			return NotFound();
		}

		return Ok(updatedUserInfo);
	}

	/// <summary>
	/// Creates a user profile for the currently authenticated user.
	/// <para><b>Authorization:</b> Required. The user must be authenticated.</para>
	/// </summary>
	/// <param name="newInfo">The profile information to create for the user.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> indicating the result of the profile creation.
	/// Returns 200 OK if successful, 400 BadRequest if a profile already exists,
	/// 404 NotFound if the user is not found, or 401 Unauthorized if the user ID is missing.
	/// </returns>
	[Authorize]
	[HttpPost("create-my-profile")]
	public async Task<IActionResult> CreateProfileForCurrentUser([FromBody] UserInfoPostDto newInfo)
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (userId == null)
			return Unauthorized("UserId is not found in token.");

		var user = await _context.Users
			.Include(u => u.UserInfo)
			.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

		if (user == null)
			return NotFound("User was not found in database.");

		if (user.UserInfo != null)
			return BadRequest("Profile already exists.");

		var userInfo = new UserInfo
		{
			UserId = user.Id,
			FirstName = newInfo.FirstName,
			LastName = newInfo.LastName,
			Address = newInfo.Address,
			PostalCode = newInfo.PostalCode ?? string.Empty,
			City = newInfo.City ?? string.Empty,
			Country = newInfo.Country ?? string.Empty,
			PhoneNumber = newInfo.PhoneNumber,
			DateOfBirth = newInfo.DateOfBirth,
			SpecialRequests = newInfo.SpecialRequests ?? string.Empty
		};

		user.UserInfo = userInfo;
		_context.UserInfos.Add(userInfo);
		await _context.SaveChangesAsync();

		return Ok(new
		{
			Message = "Profile created successfully"
		});
	}
}