using System.ComponentModel.DataAnnotations;

namespace Blazor.Models.ViewModels;

public class CurrentUserProfileViewModel
{
	[Required(ErrorMessage = "First name is required.")]
	[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
	public string FirstName { get; set; } = "";

	[Required(ErrorMessage = "Last name is required.")]
	[StringLength(80, ErrorMessage = "Last name cannot exceed 80 characters.")]
	public string LastName { get; set; } = "";

	[Required(ErrorMessage = "Date of birth is required.")]
	[DataType(DataType.Date)]
	[Display(Name = "Date of Birth")]
	public DateOnly? DateOfBirth { get; set; }

	[Required(ErrorMessage = "Address is required.")]
	[StringLength(200, ErrorMessage = "Cannot be longer than 200 characters.")]
	public string Address { get; set; } = "";

	[Required(ErrorMessage = "Postal code is required.")]
	[StringLength(20, ErrorMessage = "Cannot be longer than 20 characters.")]
	public string? PostalCode { get; set; }

	[StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
	public string? City { get; set; }

	[StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
	public string? Country { get; set; }

	[Phone(ErrorMessage = "Invalid format for a phone number.")]
	[StringLength(32, ErrorMessage = "Phone number cannot exceed 32 characters.")]
	public string PhoneNumber { get; set; } = "";

	[StringLength(500, ErrorMessage = "Special requests cannot exceed 500 characters.")]
	public string? SpecialRequests { get; set; }
	public string? CreatedAtFormated { get; set; }
	public DateTime? CreatedAt { get; set; }
	public string? LastUpdatedFormated { get; set; }
	[Display(Name = "Last Updated")]
	public DateTime? LastUpdated { get; set; }
}