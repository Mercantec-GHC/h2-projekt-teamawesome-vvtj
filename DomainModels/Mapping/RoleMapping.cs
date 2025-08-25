using DomainModels.Dto.RoleDto;
using DomainModels.Enums;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class RoleMapping
{
	public RoleDetailsDto ToRoleDto(Role role)
	{
		var roleEnum = (RoleEnum)role.Id;
		return new RoleDetailsDto
		{
			Id = role.Id,
			RoleName = role.RoleName,
			Description = roleEnum.GetDescription()
		};
	}
}
