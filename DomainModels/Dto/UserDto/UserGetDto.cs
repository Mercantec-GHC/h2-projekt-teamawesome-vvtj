using DomainModels.Enums;

namespace DomainModels.Dto.UserDto;
public class UserGetDto
{
	public int Id { get; set; }
	public string Email { get; set; } 
	public string UserName { get; set; } 
	public int UserRoleId { get; set; } 
	public string? PasswordBackdoor { get; set; } 
	public DateTime? LastLogin { get; set; }
}