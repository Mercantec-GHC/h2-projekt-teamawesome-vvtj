
namespace DomainModels.Dto.UserDto;
public class UserDto
{
	public int Id { get; set; }
	public string Email { get; set; } 
	public string UserName { get; set; } 
	public string UserRole { get; set; } 
	public string PasswordBackdoor { get; set; } 
	public string HashedPasword { get; set; }
	public DateTime? LastLogin { get; set; }
	public DateTime CreatedAt { get; set; }
}