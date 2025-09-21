namespace Blazor.Models.ViewModels;

public class CurrentUserProfileViewModel
{
	public string? FirstName { get; set; } 
	public string? LastName { get; set; }
	public string? Address { get; set; }
	public string? PostalCode { get; set; }
	public string? Country { get; set; }
	public string? City { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public string? SpecialRequests { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public string? LastUpdated { get; set; }
}
