namespace DomainModels.Dto.RoleDto;

public class RolePostDto
{
	public int Id { get; set; }
	public required string RoleName { get; set; } = default!;
}
