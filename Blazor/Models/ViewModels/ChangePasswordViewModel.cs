using System.ComponentModel.DataAnnotations;
using DomainModels.Dto.UserDto;

namespace Blazor.Models.ViewModels;

public class ChangePasswordViewModel
{
	[Required(ErrorMessage = "This field is required")]
	[DataType(DataType.Password)]
	public string NewPassword { get; set; } = string.Empty;

	[Required(ErrorMessage = "This field is required")]
	[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
	[DataType(DataType.Password)]
	public string ConfirmNewPassword { get; set; } = string.Empty;
}

public static class ChangePasswordViewModelExtended 
{
	public static ChangePasswordDto ToChangePasswordDto(this ChangePasswordViewModel vm)
	{
		return new ChangePasswordDto
		{
			NewPassword = vm.NewPassword,
			ConfirmPassword = vm.ConfirmNewPassword
		};
	}
}