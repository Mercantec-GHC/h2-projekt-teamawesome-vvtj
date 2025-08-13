namespace DomainModels;

public class UserInfo:Common
{
	public required string FirstName { get; set; } = string.Empty;
	public required string LastName { get; set; } = string.Empty;
	public required string Address { get; set; } = string.Empty;
	public required string PostalCode { get; set; } = string.Empty;
	public required string City { get; set; } = string.Empty;
	public required string Country { get; set; } = string.Empty;
	public required string PhoneNumber { get; set; } = string.Empty;
	public int UserId { get; set; }
	public User? User { get; set; } 
}
