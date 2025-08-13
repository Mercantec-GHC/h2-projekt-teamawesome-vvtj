namespace DomainModels.Dto.UserDto;
public class UserPostDto
{
	public int Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public int UserRoleId { get; set; }
}
