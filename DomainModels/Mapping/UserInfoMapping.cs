using DomainModels.Dto;
using DomainModels.Models;

namespace DomainModels.Mapping;

public static class UserInfoMapping
{
	public static UserInfoGetDto ToUserInfoGetDto(this UserInfo ui)
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
			SpecialRequests = ui.SpecialRequests
		};
	}

	public static UserInfo FromDtoToUserInfo(UserInfoPostDto dto)
	{
		return new UserInfo
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			Address = dto.Address,
			PostalCode = dto.PostalCode ?? string.Empty,
			City = dto.City ?? string.Empty,
			Country = dto.Country ?? string.Empty,
			PhoneNumber = dto.PhoneNumber,
			DateOfBirth = dto.DateOfBirth,
			SpecialRequests = dto.SpecialRequests ?? string.Empty
		};
	}

	public static void UpdateUserInfoFromDto(UserInfo existing, UserInfoPutDto dto)
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
