namespace Blazor.Models.Dto.Auth;

public class LoginResponseDto
{
	public string Token { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; }
	//public UserInfoDto? User { get; set; }
}
