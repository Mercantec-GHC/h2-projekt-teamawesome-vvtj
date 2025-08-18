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
			LastLogin = (DateTimeOffset?)user.LastLogin,
			Role = user.UserRole != null ? user.UserRole.RoleName : RoleEnum.Unknown
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
			UserRoleId = userPostDto.UserRoleId,
			PasswordBackdoor = string.Empty,
			CreatedAt = utcNow,
			UpdatedAt = utcNow,
			LastLogin = utcNow,
		};
	}
}
