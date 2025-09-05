using DomainModels.Dto;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailFormDto dto);
    }
}
