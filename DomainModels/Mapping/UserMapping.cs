using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class UserMapping
{
	public UserGetDto ToUserGetDto(User user)
	{
		return new UserGetDto
		{
			Id = user.Id,
			Email = user.Email,
			UserName = user.UserName,
			LastLogin = user.LastLogin ?? DateTime.UtcNow.AddHours(2),
			PasswordBackdoor = user.HashedPassword,
			UserRoleId = user.UserRoleId ?? 1
		};
	}

	public User ToUserFromDto(UserPostDto userPostDto)
	{
		var utcNow = DateTime.UtcNow.AddHours(2);
		return new User
		{
			Email = userPostDto.Email,
			UserName = userPostDto.UserName,
			HashedPassword = userPostDto.NewPassword,
			Salt = string.Empty,
			UserRoleId = (int)RoleEnum.Unknown,
			CreatedAt = utcNow,
			UpdatedAt = utcNow,
			LastLogin = utcNow,
		};
	}
}
