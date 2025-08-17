namespace DomainModels.Dto;

public class RoleDto
{
	public int Id { get; set; }
	public string RoleName { get; set; } = string.Empty;
	public DateTime? CreatedAt { get; set; } 
	public DateTime? UpdatedAt { get; set; }
}
