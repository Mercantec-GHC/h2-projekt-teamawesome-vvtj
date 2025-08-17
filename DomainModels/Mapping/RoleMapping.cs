using DomainModels.Dto;
using DomainModels.Models;

namespace DomainModels.Mapping;

public class RoleMapping
{
	public RoleDto ToRoleGetDto(Role role)
	{
		var utcNow = DateTime.UtcNow.AddHours(2);
		return new RoleDto
		{
			Id = role.Id,
			RoleName = role.RoleName.ToString(),
			CreatedAt = utcNow,
			UpdatedAt = utcNow
		};
	}
}
