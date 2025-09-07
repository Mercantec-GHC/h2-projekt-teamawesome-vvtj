using System.ComponentModel.DataAnnotations;

namespace Blazor.Models.Dto.Auth;

public class UserRegisterDto
{
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Wrong format for email address")]
	[Display(Name = "Email")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Username is required")]
	[MinLength(4, ErrorMessage = "Username must contain minimum 4 characters.")]
	[MaxLength(32, ErrorMessage = "Username cannot be longer than 32 characters.")]
	[RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage =
	"Only letters, numbers, _, . and - are accepted")]
	[Display(Name = "Username")]
	public string Username { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required")]
	[StringLength(100, MinimumLength = 8,
	ErrorMessage = "Password must have at least 8 characters")]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;

	[Required(ErrorMessage = "Repeat password")]
	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "Password doesn't match")]
	[Display(Name = "Confirm password")]
	public string ConfirmPassword { get; set; } = string.Empty;

}
