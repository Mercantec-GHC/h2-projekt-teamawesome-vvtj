using DomainModels.Dto;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class RoleMapping
{
	public RoleDto ToRoleGetDto(Role role)
	{
		return new RoleDto
		{
			Id = role.Id,
			RoleName = role.RoleName.ToString()
		};
	}
}
