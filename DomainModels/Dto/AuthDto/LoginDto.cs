﻿using System.ComponentModel.DataAnnotations;

namespace DomainModels.Dto.AuthDto;

public class LoginDto
{
	[Required(ErrorMessage = "Username is required")]
	[Display(Name = "Username")]
	public string Username { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required")]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;
}
