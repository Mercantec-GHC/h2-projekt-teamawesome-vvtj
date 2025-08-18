using DomainModels.Enums;

namespace DomainModels.Dto.UserDto;
public class UserGetDto
{
	public int? Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public RoleEnum Role { get; set; }
	public DateTimeOffset? LastLogin { get; set; }
}