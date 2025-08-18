using DomainModels.Enums;

namespace DomainModels.Dto.UserDto;
public class UserPostDto
{ 
	public string Email { get; set; }
	public string UserName { get; set; }
	public string NewPassword { get; set; }
	public RoleEnum? UserRole { get; set; }

}
