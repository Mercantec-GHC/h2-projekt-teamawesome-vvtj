using DomainModels.Enums;

namespace DomainModels.Dto.RoleDto;

public class RoleDetailsDto
{
	public int Id { get; set; }
	public RoleEnum RoleName { get; set; }
	public string? Description { get; set; } 
}
