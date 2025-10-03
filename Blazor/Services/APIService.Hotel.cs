using DomainModels.Dto;
using System.Net.Http.Json;

namespace Blazor.Services
{
    public partial class APIService
    {
        public async Task<IEnumerable<HotelDto>?> GetAllHotelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Hotel");
                response.EnsureSuccessStatusCode();
                var results = await response.Content.ReadFromJsonAsync<List<HotelDto>>();
                return results ?? Enumerable.Empty<HotelDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching the list of rooms to clean from the API: {Message}", ex.Message);
                return null;
            }
        }
    
     public async Task<List<HotelDto>> GetHotelsAsync()
        {
            var resp = await _httpClient.GetAsync("/api/Hotel");
            if (!resp.IsSuccessStatusCode) return new();
            return await resp.Content.ReadFromJsonAsync<List<HotelDto>>() ?? new();
        }
    }
}