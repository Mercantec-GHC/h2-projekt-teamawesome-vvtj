using DomainModels.Enums;

namespace DomainModels.Models;
public class Role : Common
{
	public RoleEnum RoleName { get; set; }
	public ICollection<User> Users { get; set; } = new List<User>();
}