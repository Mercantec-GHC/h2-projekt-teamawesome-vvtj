namespace DomainModels.Dto.UserDto;

public class AssignRoleDto
{
	public int UserId { get; set; }
	public int RoleId { get; set; }
	public string? AssignedBy { get; set; } 
}
