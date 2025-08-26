using DomainModels.Dto;
using System.Net.Http.Json;

namespace Blazor.Services
{
    public partial class APIService
    {
        public async Task<IEnumerable<RoomToCleanDto>?> GetAllRoomsToClean()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Cleaning");
                response.EnsureSuccessStatusCode();
                var results = await response.Content.ReadFromJsonAsync<List<RoomToCleanDto>>();
                return results ?? Enumerable.Empty<RoomToCleanDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching the list of rooms to clean from the API: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<bool> MarkRoomsAsCleanedAsync(List<int> roomNumbers)
        {
            if (roomNumbers == null || !roomNumbers.Any())
            {
                return false;
            }
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Cleaning/MarkRoomAsCleaned", roomNumbers);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while marking rooms as cleaned: {Message}", ex.Message);
                return false;
            }
        }

    }
}
