using DomainModels.Enums;

namespace DomainModels.Models;
public class User : Common
{
	public required string Email { get; set; }
	public required string UserName { get; set; }
	public required string HashedPassword { get; set; }
	public DateTime? LastLogin { get; set; }
	public int UserRoleId { get; set; } = (int)RoleEnum.Guest;
	public Role UserRole { get; set; } 
	public int? UserInfoId { get; set; }
	public UserInfo? UserInfo { get; set; } 
	public List<Booking>? Bookings { get; set; } = new List<Booking>();
	public string? PasswordBackdoor { get; set; }
	// Only for educational purposes, not in the final product!
	public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
}