using DomainModels.Dto.UserDto;
using DomainModels.Dto.UserProfileDto;

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

	public static CurrentUserProfileViewModel ToViewModel(this UserInfoGetDto dto)
	{
		return new CurrentUserProfileViewModel
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			Address = dto.Address,
			PostalCode = dto.PostalCode,
			City = dto.City,
			Country = dto.Country,
			PhoneNumber = dto.PhoneNumber,
			DateOfBirth = dto.DateOfBirth,
			SpecialRequests = dto.SpecialRequests,
			LastUpdatedFormated = dto.UpdatedAt?.ToString("dd-MM-yyyy HH:mm"),
			UpdatedAt = dto.UpdatedAt
		};
	}
}