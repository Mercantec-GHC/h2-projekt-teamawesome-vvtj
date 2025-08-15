namespace DomainModels.Dto.UserDto;
public class UserPostDto
{
	public string Email { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public int UserRoleId { get; set; }
}
