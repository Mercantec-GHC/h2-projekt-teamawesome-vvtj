using System.Net.Http.Json;
using DomainModels.Dto;

namespace Blazor.Services
{
    public partial class APIService
    {
       public async Task<bool> SendEmailAsync(EmailFormDto dto)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync("api/Email/Sendmail", dto);
				response.EnsureSuccessStatusCode();

				if (response.IsSuccessStatusCode)
				{
					await SendNotificationAsync("Someone submitted the contact form — check your email!");
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
