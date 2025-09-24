using API.Interfaces;
using DomainModels.Dto;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<CleaningService> _logger;

        public EmailService( ILogger<CleaningService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailFormDto dto)
        {
            try
            {
                // Create the email message, using fake SMTP settings for demonstration (e.g., Mailtrap)
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                     $"Name: {dto.Name}",
                     "test@example.com"
                 ));
                email.To.Add(MailboxAddress.Parse(dto.Email));
                email.Subject = "Email from NOVA Hotels website";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"Email: {dto.Email} ; Message: {dto.Message}"
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("sandbox.smtp.mailtrap.io", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("a7342b2a3cc877", "5f0cabeff0b224");
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
				var email = new MimeMessage();

				email.From.Add(new MailboxAddress("NOVA Hotels Team", "noreply@novahotels.com"));
				email.To.Add(MailboxAddress.Parse(dto.Email));
				email.Subject = "Welcome to NOVA Hotels!";

				email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
				{
					Text = $@"
                    <h1>Congratulations, {dto.Name}!</h1>
                    <p>Thank you for registering with NOVA Hotels. Your account has been successfully created.</p>
                    <p>You can now log in and start booking rooms.</p>
                    <a href='[YOUR LOGIN LINK]'>Log in</a>
                    "};

				using var smtp = new SmtpClient();
				await smtp.ConnectAsync("sandbox.smtp.mailtrap.io", 587, SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync("a7342b2a3cc877", "5f0cabeff0b224"); 
				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"[Welcome Email Error for {dto.Email}]: {ex.Message}");
				return false;
			}
		}
	}
}
