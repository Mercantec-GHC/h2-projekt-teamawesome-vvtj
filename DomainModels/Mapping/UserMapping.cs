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
			CreatedAt = user.CreatedAt,
			PasswordBackdoor = user.PasswordBackdoor ?? string.Empty,
			UserRole = user.UserRole.RoleName.ToString()
		};
	}

	public User ToUserFromDto(UserPostDto userPostDto)
	{
		var utcNow = DateTime.UtcNow.AddHours(2);
		return new User
		{
			Email = userPostDto.Email,
			UserName = userPostDto.UserName,
<<<<<<< Updated upstream
			HashedPassword = userPostDto.NewPassword,
			UserRoleId = (int)RoleEnum.Unknown,
			UpdatedAt = utcNow,
		};
	}
}
