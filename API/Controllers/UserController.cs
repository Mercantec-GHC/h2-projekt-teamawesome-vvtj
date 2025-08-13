using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    IUserService _userService;
    
    public UserController(IUserService apiService)
    {
        _userService = apiService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto) {
        if (userDto == null)
        {
            return BadRequest("User data is required.");
        }
        try
        {
            var result = await _userService.CreateUserAsync(userDto);
            if (result)
            {
                return Ok("User created successfully.");
            }
            else
            {
                return BadRequest("Failed to create user.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
