using DomainModels.Dto.UserProfileDto;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class UserInfoMapping
{
	public UserInfoGetDto ToUserInfoGetDto(UserInfo ui)
	{
		return new UserInfoGetDto
		{
			UserId = ui.UserId,
			FirstName = ui.FirstName,
			LastName = ui.LastName,
			Address = ui.Address,
			PostalCode = ui.PostalCode,
			City = ui.City,
			Country = ui.Country,
			PhoneNumber = ui.PhoneNumber,
			DateOfBirth = ui.DateOfBirth,
			SpecialRequests = ui.SpecialRequests,
			CreatedAt = ui.CreatedAt,
			UpdatedAt = DateTime.UtcNow.AddHours(2)
		};
	}

	public void UpdateUserInfoFromDto(UserInfo existing, UserInfoPutDto dto)
	{
		if (!string.IsNullOrWhiteSpace(dto.FirstName))
			existing.FirstName = dto.FirstName;

		if (!string.IsNullOrWhiteSpace(dto.LastName))
			existing.LastName = dto.LastName;

		if (!string.IsNullOrWhiteSpace(dto.Address))
			existing.Address = dto.Address;

		if (!string.IsNullOrWhiteSpace(dto.PostalCode))
			existing.PostalCode = dto.PostalCode;

		if (!string.IsNullOrWhiteSpace(dto.City))
			existing.City = dto.City;

		if (!string.IsNullOrWhiteSpace(dto.Country))
			existing.Country = dto.Country;

		if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
			existing.PhoneNumber = dto.PhoneNumber;

		if (dto.DateOfBirth.HasValue)
			existing.DateOfBirth = dto.DateOfBirth.Value;

		if (!string.IsNullOrWhiteSpace(dto.SpecialRequests))
			existing.SpecialRequests = dto.SpecialRequests;
	}
}
