using System.ComponentModel.DataAnnotations;

namespace DomainModels.Dto.UserDto;

public class ChangePasswordDto
{
	[Required]
	[DataType(DataType.Password)]
	public string NewPassword { get; set; }
	[Required]
	[DataType(DataType.Password)]
	[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
	public string ConfirmPassword { get; set; }

}
