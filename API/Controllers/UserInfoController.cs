using API.Interfaces;
using DomainModels.Dto.UserProfileDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Provides API endpoints for managing user profile information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
	private readonly IUserInfoService _userInfoService;

	public UserInfoController(IUserInfoService userInfoService)
	{
		_userInfoService = userInfoService;
	}

	/// <summary>
	/// Retrieves user profile information by user ID.
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> containing the user profile information if found;
	/// otherwise, a 404 Not Found response.
	/// </returns>
	/// 
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
	/// Updates user profile information for a specific user.
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <param name="updatedInfo">The updated user profile data.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> containing the updated user profile information if successful;
	/// otherwise, a 404 Not Found response.
	/// </returns>
	/// 
	[Authorize]
	[HttpPut("{userId}")]
	public async Task<IActionResult> UpdateUserInfo(int userId, [FromBody] UserInfoPutDto updatedInfo)
	{
		var updatedUserInfo = await _userInfoService.UpdateUserInfoAsync(userId, updatedInfo);
		if (updatedUserInfo == null)
		{
			return NotFound();
		}
		return Ok(updatedUserInfo);
	}

	/// <summary>
	/// Creates new user profile information.
	/// </summary>
	/// <param name="newInfo">The user profile data to create.</param>
	/// <returns>
	/// An <see cref="IActionResult"/> containing the created user profile information if successful;
	/// otherwise, a 400 Bad Request response.
	/// </returns>
	/// 
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> CreateUserInfo([FromBody] UserInfoPostDto newInfo)
	{
		var createdUserInfo = await _userInfoService.CreateUserInfoAsync(newInfo);
		if (createdUserInfo == null)
		{
			return BadRequest("Could not create user info.");
		}
		return CreatedAtAction(nameof(GetUserInfoByUserId), new { userId = createdUserInfo.UserId }, createdUserInfo);
	}
}