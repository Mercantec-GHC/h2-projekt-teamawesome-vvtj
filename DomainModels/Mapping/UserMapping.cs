using DomainModels.Dto.UserDto;
using DomainModels.Enums;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class UserMapping
{
	public UserDto ToUserGetDto(User user)
	{
		return new UserDto
		{
			Id = user.Id,
			Email = user.Email,
			UserName = user.UserName,
			LastLogin = user.LastLogin ?? DateTime.UtcNow.AddHours(2),
			CreatedAt = user.CreatedAt,
			UpdatedAt = DateTime.UtcNow.AddHours(2),
			HashedPasword = user.HashedPassword,
			UserRole = user.UserRole.RoleName.ToString(),
		};
	}
}
