using System.Net;
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
                var response = await GetAsync("api/RoomTypes");
                response.EnsureSuccessStatusCode();

                //Returns deserialised content 
                var roomTypes = await response.Content.ReadFromJsonAsync<IEnumerable<RoomTypeDto>>();
                return roomTypes;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error finding RoomTypes: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occured with call: {ex.Message}");
                throw;
            }
           
        }
    }
}
