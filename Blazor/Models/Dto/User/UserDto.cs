using DomainModels.Dto.UserProfileDto;

namespace Blazor.Models.Dto.User;

public class UserDto
{
	public int Id { get; set; }
	public string Email { get; set; }
	public string UserName { get; set; }
	public string UserRole { get; set; }
	public string HashedPasword { get; set; }
	public UserInfoDto? UserInfo { get; set; }
	public DateTime? LastLogin { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
