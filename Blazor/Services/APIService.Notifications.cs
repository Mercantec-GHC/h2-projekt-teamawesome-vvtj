using System.Net.Http.Json;
using DomainModels.Dto;

namespace Blazor.Services
{
	public partial class APIService
	{
		public async Task<bool> SendNotificationAsync(string message)
		{
			try
			{
				var request = new NotificationMessageDto { Message = message };
				var response = await PostAsJsonAsync("api/Notifications/send", request);
				response.EnsureSuccessStatusCode();
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while sending push notification: {Message}", ex.Message);
				return false;
			}
		}

		public async Task<bool> SendEmailAsync(EmailFormDto dto)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync("api/Notifications/save-contact-form-notification", dto);
				response.EnsureSuccessStatusCode();

				if (response.IsSuccessStatusCode)
				{
					await SendNotificationAsync("Someone submitted the contact form — check notifications!");
				}

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while sending contact form: {Message}", ex.Message);
				return false;
			}
		}
	}
}
