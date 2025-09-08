using DomainModels.Dto.UserDto;

namespace Blazor.Models.ViewModels;

public static class CurrentUserViewModelExtensions
{
	public static CurrentUserAccountViewModel ToViewModel(this UserDto dto)
	{
		return new CurrentUserAccountViewModel
		{
			Email = dto.Email,
			Username = dto.UserName,
			Role = dto.UserRole,
			CreatedAt = dto.CreatedAt,
			CreatedAtFormated = dto.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
			LastLogin = dto.LastLogin,
			LastLoginFormated = dto.LastLogin?.ToString("dd-MM-yyyy HH:mm") ?? "Never",
			UserInfo = dto.UserInfo
		};
	}
}