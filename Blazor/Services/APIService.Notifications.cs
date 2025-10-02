using System.Net.Http.Json;
using DomainModels.Dto;

namespace Blazor.Services
{
	public partial class APIService
	{
		/// <summary>
		/// Sends a push notification message to the backend API.
		/// </summary>
		/// <param name="message">The notification message to send.</param>
		/// <returns>True if the notification was sent successfully; otherwise, false.</returns>
		public async Task<bool> SendNotificationAsync(string message)
		{
			try
			{
				// Prepare the notification DTO
				var request = new NotificationMessageDto { Message = message };
				// Send the notification to the API endpoint
				var response = await PostAsJsonAsync("api/Notifications/send", request);
				response.EnsureSuccessStatusCode();
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				// Log any errors encountered during the notification process
				_logger.LogError(ex, "Exception while sending push notification: {Message}", ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Sends an email using the contact form data and triggers a notification if successful.
		/// </summary>
		/// <param name="dto">The contact form data transfer object.</param>
		/// <returns>True if the email was sent successfully; otherwise, false.</returns>
		public async Task<bool> SendEmailAsync(EmailFormDto dto)
		{
			try
			{
				// Send the contact form data to the API endpoint
				var response = await _httpClient.PostAsJsonAsync("api/Notifications/save-contact-form-notification", dto);
				response.EnsureSuccessStatusCode();

				if (response.IsSuccessStatusCode)
				{
					// Trigger a notification if the email was sent successfully
					await SendNotificationAsync("Someone submitted the contact form — check notifications!");
				}

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				// Log any errors encountered during the email sending process
				_logger.LogError(ex, "Exception while sending contact form: {Message}", ex.Message);
				return false;
			}
		}
	}
}
