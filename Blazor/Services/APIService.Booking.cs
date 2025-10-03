using DomainModels.Dto;
using System.Net;
using System.Net.Http.Json;


namespace Blazor.Services
{
    public partial class APIService
    {
        public async Task<BookingResponseDto?> CreateBooking(CreateBookingDto dto, bool preview = false)
        {
            try
            {
                var url = $"api/bookings?preview={(preview ? "true" : "false")}";
                var response = await PostAsJsonAsync(url, dto);

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

        public async Task<List<BookingByUserDto>> GetBookingsByCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/bookings");

                if (response.IsSuccessStatusCode)
                {
                    var bookings = await response.Content.ReadFromJsonAsync<List<BookingByUserDto>>();
                    return bookings ?? new List<BookingByUserDto>();
                }

                _logger.LogWarning("Failed to fetch bookings. Status: {StatusCode}", response.StatusCode);
                return new List<BookingByUserDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error fetching bookings.");
                return new List<BookingByUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error fetching bookings.");
                throw;
            }

        }
        public async Task<bool> DeleteMyBookingAsync(int Id)
        {
            var response = await _httpClient.DeleteAsync($"/api/bookings/user/bookings/{Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<GetAvailableRoomsDto>> GetAvailableRoomsAsync(
            string hotelName, DateOnly from, DateOnly to)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/bookings/available?hotelName={hotelName}&from={from}&to={to}");


                if (response.IsSuccessStatusCode)
                {
                    var rooms = await response.Content
                        .ReadFromJsonAsync<List<GetAvailableRoomsDto>>();
                    return rooms ?? new List<GetAvailableRoomsDto>();
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new List<GetAvailableRoomsDto>(); // empty list if rooms not found

                _logger.LogWarning("Failed to get available rooms. Status: {Status}", response.StatusCode);
                return new List<GetAvailableRoomsDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error getting available rooms.");
                return new List<GetAvailableRoomsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error getting available rooms.");
                throw;
            }
        }

       
    }
}
