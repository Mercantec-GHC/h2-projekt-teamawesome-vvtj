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
	}
}
