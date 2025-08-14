namespace DomainModels.Dto.RoleDto;

public class RoleGetDto
{
	public int Id { get; set; }
	public string? RoleName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
