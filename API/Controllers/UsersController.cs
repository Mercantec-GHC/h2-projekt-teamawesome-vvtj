using API.Interfaces;
using DomainModels.Dto.UserDto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserGetDto>>> GetUsers()
	{
		var users = await _userService.GetAllUsersAsync();
		if(users == null || !users.Any())
		{
			return NotFound("No users found.");
		}
		return Ok(users);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserGetDto>> GetUserById(int id)
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
		//return CreatedAtAction(nameof(GetUserById), createdUser);
		return Ok();
	}
	[HttpPut]
	public async Task<IActionResult> UpdateUser([FromBody] UserPostDto userDto)
	{
		if (userDto == null)
		{
			return BadRequest("User data is null.");
		}
		var updated = await _userService.UpdateUserAsync(userDto);
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
