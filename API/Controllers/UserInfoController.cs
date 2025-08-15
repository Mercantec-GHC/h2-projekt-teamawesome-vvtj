using System.Security.Claims;
using API.Interfaces;
using DomainModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
	private readonly IUserInfoService _userInfoService;

	public UserInfoController(IUserInfoService userInfoService)
	{
		_userInfoService = userInfoService;
	}

	//[HttpGet("me")]
	//public async Task<IActionResult> GetMyInfo()
	//{
	//	var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
	//	var info = await _userInfoService.GetByUserIdAsync(userId);
	//	if (info == null) return NotFound();
	//	return Ok(info);
	//}

	//[HttpPut("me")]
	//public async Task<IActionResult> UpdateMyInfo([FromBody] UserInfo updated)
	//{
	//	var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
	//	var result = await _userInfoService.UpdateUserInfoAsync(userId, updated);
	//	if (result == null) return NotFound();
	//	return Ok(result);
	//}

	[HttpGet("{userId}")]
	public async Task<IActionResult> GetUserInfo(int userId)
	{
		var info = await _userInfoService.GetByUserIdAsync(userId);
		if (info == null) return NotFound();
		return Ok(info);
	}

	[HttpPut("{userId}")]
	public async Task<IActionResult> UpdateUserInfo(int userId, [FromBody] UserInfo updated)
	{
		var result = await _userInfoService.UpdateUserInfoAsync(userId, updated);
		if (result == null) return NotFound();
		return Ok(result);
	}
}