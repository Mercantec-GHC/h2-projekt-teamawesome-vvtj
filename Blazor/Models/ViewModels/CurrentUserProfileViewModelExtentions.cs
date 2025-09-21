using DomainModels.Dto.UserProfileDto;

namespace Blazor.Models.ViewModels;

public static class CurrentUserProfileViewModelExtentions
{
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
			CreatedAtFormated = dto.CreatedAt.ToString("dd-mm-yyyy HH:mm:ss"),
			CreatedAt = dto.CreatedAt,
			LastUpdatedFormated = dto.UpdatedAt?.ToString("dd-mm-yyyy HH:mm:ss"),
			LastUpdated = dto.UpdatedAt
		};
	}

	public static UserInfoPutDto ToPutDto(this CurrentUserProfileViewModel vm)
	{
		return new UserInfoPutDto
		{
			FirstName = vm.FirstName,
			LastName = vm.LastName,
			Address = vm.Address,
			PostalCode = vm.PostalCode,
			City = vm.City,
			Country = vm.Country,
			PhoneNumber = vm.PhoneNumber,
			DateOfBirth = vm.DateOfBirth,
			SpecialRequests = vm.SpecialRequests,
			UpdatedAt = DateTime.UtcNow.AddHours(2)
		};
	}

	public static UserInfoPostDto UserInfoPostDto(this CurrentUserProfileViewModel vm, int userId)
	{
		return new UserInfoPostDto
		{
			FirstName = vm.FirstName,
			LastName = vm.LastName,
			Address = vm.Address,
			PostalCode = vm.PostalCode,
			City = vm.City,
			Country = vm.Country,
			PhoneNumber = vm.PhoneNumber,
			DateOfBirth = vm.DateOfBirth,
			SpecialRequests = vm.SpecialRequests,
			CreatedAt = DateTime.UtcNow.AddHours(2)
		};
	}
}
