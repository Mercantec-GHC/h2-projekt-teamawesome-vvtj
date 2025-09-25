using DomainModels.Dto;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailNotificationAsync(EmailFormDto dto);
        Task<bool> SendWelcomeEmailAsync(EmailFormDto dto);
	}
}
