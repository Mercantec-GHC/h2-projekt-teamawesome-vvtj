using System.ComponentModel.DataAnnotations;

namespace DomainModels.Dto.UserProfileDto;

public class UserInfoGetDto
{
	public int UserId { get; set; }
	[Required, MaxLength(50)]
	public string FirstName { get; set; } = "";
	[Required, MaxLength(80)]
	public string LastName { get; set; } = "";
	[Required, MaxLength(200)]
	public string Address { get; set; } = "";
	[Required, MaxLength(20)]
	public string? PostalCode { get; set; }
	[MaxLength(100)]
	public string? City { get; set; }
	[MaxLength(100)]
	public string? Country { get; set; }
	[Phone, Required, MaxLength(32)]
	public string PhoneNumber { get; set; } = "";
	public DateOnly? DateOfBirth { get; set; }
	[MaxLength(500)]
	public string? SpecialRequests { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
