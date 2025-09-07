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
			CreatedAtFormated = dto.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
			LastLogin = dto.LastLogin,
			LastLoginFormated = dto.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "Never",
			UserInfo = dto.UserInfo
		};
	}
}