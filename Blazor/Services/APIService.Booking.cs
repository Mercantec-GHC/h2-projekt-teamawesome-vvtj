using DomainModels.Dto;
using System.Net;
using System.Net.Http.Json;


namespace Blazor.Services
{
    public partial class APIService
    {
        public async Task<BookingResponseDto?> CreateBooking(CreateBookingDto dto)
        {
            try
            {
                var response = await PostAsJsonAsync("api/bookings", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BookingResponseDto>();
                    return result;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("BadRequest when creating booking.");
                    return null;
                }

                _logger.LogError("Failed to create booking. Status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error when creating booking.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error when creating booking.");
                throw;
            }
        }

        public async Task<List<RoomTypeDto>> GetAllRoomTypesAsync()
        {
            var response = await GetAsync("api/roomtypes");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<RoomTypeDto>>();
                return result ?? new List<RoomTypeDto>();
            }
            return new List<RoomTypeDto>();
        }
    }


}
