using System.Globalization;
using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;

namespace API.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<CleaningService> _logger;
		private readonly IConfiguration _config;

		private readonly AppDBContext _context;

		public EmailService(ILogger<CleaningService> logger, IConfiguration config, AppDBContext context)
		{
			_logger = logger;
			_config = config;
			_context = context;
		}

		public async Task<bool> SendWelcomeEmailAsync(EmailFormDto dto)
		{
			try
			{
				var username = _config["SmtpSettings:GmailUsername"];
				var appPassword = _config["SmtpSettings:GmailAppPassword"];

				if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(appPassword))
				{
					_logger.LogError("[Email Error]: SMTP credentials (GmailUsername or AppPassword) are missing or empty in configuration.");
					return false; 
				}

				var email = new MimeMessage();

				email.From.Add(new MailboxAddress("NOVA Hotels Team", username));
				email.To.Add(MailboxAddress.Parse(dto.Email));

				email.Subject = "Welcome to NOVA Hotels!";

				string htmlContent = $@"
					<!DOCTYPE html>
					<html>
					<head>
						<meta charset='utf-8'>
						<meta name='viewport' content='width=device-width, initial-scale=1'>
						<title>Welcome to NOVA Hotels!</title>
						<style>
							body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
							.container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 8px rgba(0,0,0,0.05); }}
							.header {{ background-color: #007bff; color: #ffffff; padding: 20px; text-align: center; }}
							.content {{ padding: 30px; text-align: center; color: #333333; }}
							.button {{ background-color: #28a745; color: #ffffff !important; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-top: 20px; }}
							.footer {{ background-color: #e9e9e9; color: #777777; padding: 15px; font-size: 12px; text-align: center; }}
							h1 {{ color: #007bff; }}
						</style>
					</head>
					<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>

						<table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f4f4f4;'>
							<tr>
								<td align='center'>
									<table role='presentation' width='600' cellspacing='0' cellpadding='0' border='0' class='container' style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 8px rgba(0,0,0,0.05);'>
										<tr>
											<td align='center' class='header' style='background-color: #007bff; color: #ffffff; padding: 20px; text-align: center;'>
												<h2 style='margin: 0; font-size: 24px;'>NOVA Hotels</h2>
											</td>
										</tr>
                    
										<tr>
											<td class='content' style='padding: 30px; text-align: center; color: #333333;'>
												<h1 style='color: #007bff; font-size: 28px;'>Welcome, {dto.Name}!</h1>
                            
												<p style='font-size: 16px; line-height: 1.6;'>
													Thank you for registering with NOVA Hotels. Your account has been successfully created.
												</p>
                            
												<p style='font-size: 16px; line-height: 1.6;'>
													You can now log in to view our best offers and book your perfect stay.
												</p>
                            
												<a href='https://prod-novahotels-blazor-mercantec-tech.azurewebsites.net/login' target='_blank' class='button' style='background-color: #28a745; color: #ffffff !important; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-top: 20px;'>
													Log In to Your Account
												</a>
											</td>
										</tr>

										<tr>
											<td class='footer' style='background-color: #e9e9e9; color: #777777; padding: 15px; font-size: 12px; text-align: center;'>
												<p style='margin: 0;'>&copy; {DateTime.Now.Year} NOVA Hotels. All rights reserved.</p>
												<p style='margin: 5px 0 0 0;'>This is an automated message. Please do not reply.</p>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>

					</body>
					</html>
					";

				email.Body = new TextPart(TextFormat.Html)
				{
					Text = htmlContent
				};

				// Sending logic remains the same
				using var smtp = new SmtpClient();
				await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync(username, appPassword);

				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"[Email Error]: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> SendBookingConfirmationEmailAsync(EmailFormDto dto, BookingResponseDto responseDto)
		{
			var username = _config["SmtpSettings:GmailUsername"];
			var appPassword = _config["SmtpSettings:GmailAppPassword"];

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(appPassword))
			{
				_logger.LogError("[Booking Confirmation Email Error]: SMTP credentials are missing or empty in configuration.");
				return false;
			}

			try
			{
				var email = new MimeMessage();

				email.From.Add(new MailboxAddress("NOVA Hotels Reservations", username));
				email.To.Add(MailboxAddress.Parse(dto.Email));
				email.Subject = $"Booking Confirmed: {responseDto.HotelName}";


				string checkInDate = responseDto.CheckIn.ToString("ddd, MMM dd, yyyy");
				string checkOutDate = responseDto.CheckOut.ToString("ddd, MMM dd, yyyy");
				string totalPrice = responseDto.TotalPrice.HasValue? responseDto.TotalPrice.Value.ToString("C", CultureInfo.GetCultureInfo("da-DK")): (0m).ToString("C", CultureInfo.GetCultureInfo("da-DK"));
				string isBreakfastText = responseDto.IsBreakfast ? "Yes (Included)" : "No";

				string htmlContent = $@"
				<!DOCTYPE html>
				<html>
				<head>
					<meta charset='utf-8'>
					<meta name='viewport' content='width=device-width, initial-scale=1'>
					<title>Booking Confirmation</title>
					<style>
						body {{ font-family: Arial, sans-serif; background-color: #f0f4f7; margin: 0; padding: 0; }}
						.container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
						.header {{ background-color: #007bff; color: #ffffff; padding: 25px; text-align: center; }}
						.content {{ padding: 30px; color: #333333; }}
						.details-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; border: 1px solid #ddd; }}
						.details-table th, .details-table td {{ padding: 12px; text-align: left; border-bottom: 1px solid #eee; }}
						.details-table th {{ background-color: #f7f7f7; font-weight: bold; color: #555; }}
						.final-price-row td {{ background-color: #d4edda; color: #155724; font-size: 18px; font-weight: bold; border-top: 2px solid #28a745; }}
						.footer {{ background-color: #e9e9e9; color: #777777; padding: 15px; font-size: 12px; text-align: center; }}
					</style>
				</head>
				<body style='font-family: Arial, sans-serif; background-color: #f0f4f7; margin: 0; padding: 0;'>

					<table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
						<tr>
							<td align='center'>
								<table role='presentation' width='600' cellspacing='0' cellpadding='0' border='0' class='container' style='max-width: 600px;'>
									<tr>
										<td align='center' class='header'>
											<h1 style='margin: 0; font-size: 26px;'>Booking Confirmed!</h1>
											<p style='margin: 5px 0 0 0; font-size: 18px;'>{responseDto.HotelName}</p>
										</td>
									</tr>
                    
									<tr>
										<td class='content'>
											<p style='font-size: 16px;'>
												Dear {responseDto.UserName},
											</p>
                            
											<p style='font-size: 16px; line-height: 1.6;'>
												Your reservation at {responseDto.HotelName} is confirmed! We look forward to welcoming you.
											</p>
                            
											<table class='details-table' cellspacing='0' cellpadding='0' border='0'>
												<thead>
													<tr><th colspan='2' style='background-color: #e9f5ff; color: #007bff;'>Reservation Details</th></tr>
												</thead>
												<tbody>
													<tr><th>Check-in Date:</th><td>{checkInDate}</td></tr>
													<tr><th>Check-out Date:</th><td>{checkOutDate}</td></tr>
													<tr><th>Nights:</th><td>{responseDto.NightsCount}</td></tr>
													<tr><th>Room Type:</th><td>{responseDto.TypeOfRoom}</td></tr>
													<tr><th>Guests:</th><td>{responseDto.GuestsCount}</td></tr>
													<tr><th>Breakfast Included:</th><td>{isBreakfastText}</td></tr>
													<tr class='final-price-row'>
														<td colspan='2' style='text-align: center;'>Total Price: {totalPrice}</td>
													</tr>
												</tbody>
											</table>
                            
											<p style='margin-top: 30px; text-align: center;'>
												<a href='https://prod-novahotels-blazor-mercantec-tech.azurewebsites.net/login' target='_blank' class='button' style='background-color: #28a745; color: #ffffff !important; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-top: 20px;'>
													Log In to Your Account
												</a>
											</p>

											<p style='margin-top: 30px; font-size: 14px; color: #777;'>
												Please bring a copy of this confirmation for check-in. Safe travels!
											</p>
										</td>
									</tr>

									<tr>
										<td class='footer'>
											<p style='margin: 0;'>&copy; {DateTime.Now.Year} NOVA Hotels. All rights reserved.</p>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>

				</body>
				</html>
				";

				email.Body = new TextPart(TextFormat.Html)
				{
					Text = htmlContent
				};

				using var smtp = new SmtpClient();
				await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync(username, appPassword);
				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"[Booking Confirmation Email Error]: Failed to send confirmation to {dto.Email}.");
				return false;
			}
		}

		public async Task<bool> SendTicketEmailAsync(EmailFormDto dto, NotificationStatusDto statusDto)
		{
			var username = _config["SmtpSettings:GmailUsername"];
			var appPassword = _config["SmtpSettings:GmailAppPassword"];

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(appPassword))
			{
				_logger.LogError("[Ticket status Error]: SMTP credentials are missing or empty in configuration.");
				return false;
			}

			try
			{
				var email = new MimeMessage();

				email.From.Add(new MailboxAddress("NOVA Hotels Support", username));
				email.To.Add(MailboxAddress.Parse(dto.Email));
				email.Subject = $"Ticket status: {statusDto.Status}";

				string message = dto.Message;

				string htmlContent = $@"
					<!DOCTYPE html>
					<html>
					<head>
						<meta charset='utf-8'>
						<meta name='viewport' content='width=device-width, initial-scale=1'>
						<title>Ticket Status Update</title>
						<style>
							body {{ font-family: Arial, sans-serif; background-color: #f0f4f7; margin: 0; padding: 0; }}
							.container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
							.header {{ background-color: #007bff; color: #ffffff; padding: 25px; text-align: center; }}
							.content {{ padding: 30px; color: #333333; }}
							.details-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; border: 1px solid #ddd; }}
							.details-table th, .details-table td {{ padding: 12px; text-align: left; border-bottom: 1px solid #eee; }}
							.details-table th {{ background-color: #f7f7f7; font-weight: bold; color: #555; }}
							.status-row td {{ background-color: #d4edda; color: #155724; font-size: 18px; font-weight: bold; border-top: 2px solid #28a745; }}
							.footer {{ background-color: #e9e9e9; color: #777777; padding: 15px; font-size: 12px; text-align: center; }}
							.message-box {{ border: 1px solid #ddd; padding: 15px; margin-top: 15px; background-color: #f9f9f9; border-radius: 5px; white-space: pre-wrap; }}
						</style>
					</head>
					<body style='font-family: Arial, sans-serif; background-color: #f0f4f7; margin: 0; padding: 0;'>

						<table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
							<tr>
								<td align='center'>
									<table role='presentation' width='600' cellspacing='0' cellpadding='0' border='0' class='container' style='max-width: 600px;'>
										<tr>
											<td align='center' class='header'>
												<h1 style='margin: 0; font-size: 26px;'>Your Ticket Status Has Been Updated</h1>
												<p style='margin: 5px 0 0 0; font-size: 18px;'>NOVA Hotels Support Team</p>
											</td>
										</tr>
                        
										<tr>
											<td class='content'>
												<p style='font-size: 16px;'>
													Dear **{dto.Name ?? "Customer"}**,
												</p>
                                
												<p style='font-size: 16px; line-height: 1.6;'>
													The status of your support ticket has been updated. Please see the details below:
												</p>
                                
												<table class='details-table' cellspacing='0' cellpadding='0' border='0'>
													<thead>
														<tr><th colspan='2' style='background-color: #e9f5ff; color: #007bff;'>Ticket Information</th></tr>
													</thead>
													<tbody>
														<tr><th>Your Email:</th><td>{dto.Email}</td></tr>
														<tr><th>Your Message:</th><td>{dto.Message}</td></tr>
														<tr class='status-row'>
															<td colspan='2' style='text-align: center;'>Current Status: **{statusDto.Status}**</td>
														</tr>
													</tbody>
												</table>
                                
												<p style='margin-top: 30px; text-align: center;'>
													<a href='https://prod-novahotels-blazor-mercantec-tech.azurewebsites.net/login' target='_blank' class='button' style='background-color: #28a745; color: #ffffff !important; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-top: 20px;'>
														View in Your Account
													</a>
												</p>

												<p style='margin-top: 30px; font-size: 14px; color: #777;'>
													If you have any further questions or require assistance, please reply to this email.
												</p>
											</td>
										</tr>

										<tr>
											<td class='footer'>
												<p style='margin: 0;'>&copy; {DateTime.Now.Year} NOVA Hotels. All rights reserved.</p>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>

					</body>
					</html>
				";

				email.Body = new TextPart(TextFormat.Html)
				{
					Text = htmlContent
				};

				using var smtp = new SmtpClient();
				await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync(username, appPassword);
				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"[Booking Confirmation Email Error]: Failed to send confirmation to {dto.Email}.");
				return false;
			}
		}
	}
}
