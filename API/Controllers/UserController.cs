using API.Data;
using API.Interfaces;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet]
	public async Task<IActionResult> GetUsers()
	{
		var users = await _userService.GetAllUsersAsync();
		return Ok(users);
	}
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetUserById(int id)
	{
		var user = await _userService.GetUserByIdAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		return Ok(user);
	}
	[HttpPost]
	public async Task<IActionResult> CreateUser([FromBody] UserPostDto userDto)
	{
		if (userDto == null)
		{
			return BadRequest("User data is null.");
		}
		var createdUser = await _userService.CreateUserAsync(userDto);
		if (createdUser == null)
		{
			return BadRequest("Failed to create user.");
		}
		return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
	}
	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateUser(int id, [FromBody] UserPostDto userDto)
	{
		if (userDto == null)
		{
			return BadRequest("User data is null.");
		}
		var updated = await _userService.UpdateUserAsync(id, userDto);
		if (!updated)
		{
			return NotFound();
		}
		return NoContent();
	}
	[HttpDelete("{email}")]
	public async Task<IActionResult> DeleteUserByEmail(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return BadRequest("Email is required.");
		}
		var deleted = await _userService.DeleteUserByEmailAsync(email);
		if (!deleted)
		{
			return NotFound();
		}
		return NoContent();
	}

}
