using DomainModels.Dto.RoleDto;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class RoleMapping
{
	public RoleGetDto ToRoleGetDto(Role role)
	{
		return new RoleGetDto
		{
			Id = role.Id,
			RoleName = role.RoleName,
		};
	}

	public Role RolePostToDto (RolePostDto relePostDto)
	{
		return new Role
		{
			Id = 0,
			RoleName = relePostDto.RoleName,
			CreatedAt = DateTime.UtcNow.AddHours(2),
			UpdatedAt = DateTime.UtcNow.AddHours(2),
		};
	}
}
