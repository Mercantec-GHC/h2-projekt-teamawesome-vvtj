using System.ComponentModel.DataAnnotations;

namespace DomainModels.Models;

public class UserInfo:Common
{
	public int UserId { get; set; }
	public User User { get; set; } = default!;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string PostalCode { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
	public DateOnly? DateOfBirth { get; set; }
	public string? SpecialRequests { get; set; }
}