
using DomainModels.Dto.UserProfileDto;

namespace Blazor.Models.ViewModels;
public class CurrentUserAccountViewModel
{
	public string Email { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;
	public string CreatedAtFormated { get; set; } = string.Empty;
	public string LastLoginFormated { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? LastLogin { get; set; }
	public UserInfoGetDto? UserInfo { get; set; }
}


