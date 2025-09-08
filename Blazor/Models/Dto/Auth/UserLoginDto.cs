using System.ComponentModel.DataAnnotations;

namespace Blazor.Models.Dto.Auth;

public class UserLoginDto
{
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Email has wrong format")]
	[Display(Name = "Email")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required")]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;
	public bool RememberMe { get; set; }
}
