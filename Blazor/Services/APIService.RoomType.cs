using System.Net.Http.Json;

namespace Blazor.Services
{
    public partial class APIService
    {

        public async Task<IEnumerable<RoomTypeDto?>?> GetRoomTypeAsync()
        {
            try
            {
                //Gets endpoint
                Console.WriteLine($"room type link: {_httpClient.BaseAddress}api/RoomTypes");

                var response = await GetAsync("api/RoomTypes");
                response.EnsureSuccessStatusCode();

                //Returns deserialised content 
                var roomTypes = await response.Content.ReadFromJsonAsync<IEnumerable<RoomTypeDto>>();
                return roomTypes;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error finding RoomTypes: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error occured with call: {ex.Message}");
                throw;
            }
           
        }
    }
}
