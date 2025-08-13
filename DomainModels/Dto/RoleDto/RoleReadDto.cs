namespace DomainModels.Dto.RoleDto;

public class RoleReadDto
{
	public int Id { get; set; }
	public string? RoleName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
