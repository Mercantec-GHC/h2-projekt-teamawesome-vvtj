using DomainModels.Dto;
using System.Net.Http.Json;

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
