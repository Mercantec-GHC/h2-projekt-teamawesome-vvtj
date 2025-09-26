using System.Globalization;
using API.Interfaces;
using DomainModels.Dto;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace API.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<CleaningService> _logger;
		private readonly IConfiguration _config;

		public EmailService(ILogger<CleaningService> logger, IConfiguration config)
		{
			_logger = logger;
			_config = config;
		}

		public async Task<bool> SendEmailNotificationAsync(EmailFormDto dto)
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

				email.From.Add(new MailboxAddress("NOVA Hotels Website", username));
				email.To.Add(MailboxAddress.Parse(username));

				email.Subject = $"NEW CONTACT: Message from {dto.Name}";

				string htmlContent = $@"
				<html>
				<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
					<div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;'>
                    
						<div style='background-color: #007bff; color: #ffffff; padding: 15px; text-align: center;'>
							<h2 style='margin: 0;'>New Contact Form Submission</h2>
						</div>
                    
						<div style='padding: 25px;'>
							<p style='font-size: 16px; color: #333;'>
								You have received a new contact message from a customer via the NOVA Hotels website.
							</p>
                        
							<table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
								<tr>
									<td style='padding: 10px; border: 1px solid #eee; background-color: #f9f9f9; width: 30%; font-weight: bold;'>Name:</td>
									<td style='padding: 10px; border: 1px solid #eee;'>{dto.Name}</td>
								</tr>
								<tr>
									<td style='padding: 10px; border: 1px solid #eee; background-color: #f9f9f9; width: 30%; font-weight: bold;'>Email:</td>
									<td style='padding: 10px; border: 1px solid #eee;'><a href='mailto:{dto.Email}'>{dto.Email}</a></td>
								</tr>
							</table>

							<h3 style='margin-top: 30px; border-bottom: 2px solid #eee; padding-bottom: 5px;'>Message:</h3>
							<p style='padding: 15px; background-color: #fff8e1; border-left: 5px solid #ffc107; white-space: pre-wrap; color: #555;'>
								{dto.Message}
							</p>
						</div>

						<div style='background-color: #e9e9e9; color: #777; padding: 10px; font-size: 12px; text-align: center;'>
							End of message.
						</div>
					</div>
				</body>
				</html>";

				email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
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
				_logger.LogError($"[Email Error]: {ex.Message}");
				return false;
			}
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

	}
}
