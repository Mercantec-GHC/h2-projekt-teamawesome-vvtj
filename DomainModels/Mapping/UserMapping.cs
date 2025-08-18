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
			LastLogin = (DateTime)user.LastLogin,
			Role = user.UserRole != null ? user.UserRole.RoleName : RoleEnum.Unknown,
			PasswordBackdoor = user.PasswordBackdoor,
		};
	}

	public User ToUserFromDto(UserPostDto userPostDto)
	{
		var utcNow = DateTime.UtcNow.AddHours(2);
		return new User
		{
			Email = userPostDto.Email,
			UserName = userPostDto.UserName,
			HashedPassword = userPostDto.Password,
			Salt = string.Empty,
			PasswordBackdoor = userPostDto.Password,
			UserRoleId = (int)RoleEnum.Unknown,
			CreatedAt = utcNow,
			UpdatedAt = utcNow,
			LastLogin = utcNow,
		};
	}
}
