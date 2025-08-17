using API.Interfaces;
using DomainModels.Dto.UserDto;
using DomainModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	// Static user instance required for password hashing.
	public static User user = new()
	{
		Email = string.Empty,
		UserName = string.Empty,
		HashedPassword = string.Empty,
		Salt = string.Empty
	};
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}
	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserGetDto>>> GetUsers()
	{
		var users = await _userService.GetAllUsersAsync();
		if (users == null || !users.Any())
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

	[HttpPost("register")]
	public async Task<ActionResult<UserPostDto>> Register(UserPostDto request)
	{
		var hashedPassword = new PasswordHasher<User>()
			.HashPassword(user, request.Password);

		user.UserName = request.UserName;
		user.Email = request.Email;
		user.HashedPassword = hashedPassword;
		user.Salt = string.Empty;

		return Ok(user);
	}

	[HttpPost("login")]
	public async Task<ActionResult<string>> Login(UserPostDto request)
	{
		//For educational purposes, this implementation is not secure for real life applications.
		if (user.UserName != request.UserName)
		{
			return BadRequest("User not found.");
		}
		if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.Password)
		== PasswordVerificationResult.Failed)
		{
			return BadRequest("Invalid password.");
		}

		string token = "Success";
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
