using System.ComponentModel.DataAnnotations;

namespace DomainModels.Dto;

public class UserInfoGetDto
{
	public int UserId { get; set; }
	[Required, MaxLength(100)]
	public string FirstName { get; set; } = "";
	[Required, MaxLength(100)]
	public string LastName { get; set; } = "";
	[Required, MaxLength(200)]
	public string Address { get; set; } = "";
	[MaxLength(20)]
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
}

public class UserInfoPostDto
{
	public int UserId { get; set; }
	[Required, MaxLength(100)]
	public string FirstName { get; set; } = "";
	[Required, MaxLength(100)]
	public string LastName { get; set; } = "";
	[Required, MaxLength(200)]
	public string Address { get; set; } = "";
	[MaxLength(20)]
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
}

public class UserInfoPutDto
{
	public int UserId { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Address { get; set; } 
	public string? PostalCode { get; set; }
	public string? City { get; set; }
	public string? Country { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public string? SpecialRequests { get; set; }
}
