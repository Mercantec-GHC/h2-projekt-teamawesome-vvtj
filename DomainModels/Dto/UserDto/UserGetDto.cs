using DomainModels.Enums;

namespace DomainModels.Dto.UserDto;
public class UserGetDto
{
	public int? Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public RoleEnum Role { get; set; } = 0;
	public DateTimeOffset? LastLogin { get; set; }
	public string PasswordBackdoor { get; set; } = string.Empty; // For educational purposes only, not in the final product!
	public string Role { get; set; } = string.Empty;
	public DateTime? LastLogin { get; set; }
}