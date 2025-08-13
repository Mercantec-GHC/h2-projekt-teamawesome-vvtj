namespace DomainModels;
public class Role : Common
{
	public required string? RoleName { get; set; }
	public ICollection<User> Users { get; set; } = new List<User>();
}