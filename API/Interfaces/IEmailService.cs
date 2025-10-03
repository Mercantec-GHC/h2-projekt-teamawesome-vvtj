using DomainModels.Dto;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(EmailFormDto dto);
        Task<bool> SendBookingConfirmationEmailAsync(EmailFormDto dto, BookingResponseDto responseDto);
        Task<bool> SendTicketEmailAsync(EmailFormDto dto, NotificationStatusDto statusDto);
	}
}
