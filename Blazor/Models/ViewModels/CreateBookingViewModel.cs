using Blazor.Pages.Booking;
using DomainModels.Dto;
using DomainModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Models.ViewModels
{
    public class CreateBookingViewModel
    {

        [Required] public string UserName { get; set; } = string.Empty;
        [Required] public string HotelName { get; set; } = string.Empty;
        [Required] public int RoomTypeId { get; set; } = 0;
        [Required] public DateTime CheckIn { get; set; }
        [Required] public DateTime CheckOut { get; set; }

        [Range(1, 6)] public int GuestsCount { get; set; } = 1;

        public int NightsCount { get; set; }
        public bool IsBreakfast { get; set; }

        public decimal? TotalPrice { get; set; }

    }
    public static class BookingMapping
    {
        public static CreateBookingDto ToCreateBookingDto(this CreateBookingViewModel vm)
        {
            return new CreateBookingDto
            {
                UserName = vm.UserName,
                HotelName = vm.HotelName,
                RoomTypeId = vm.RoomTypeId,
                CheckIn = DateOnly.FromDateTime(vm.CheckIn),
                CheckOut = DateOnly.FromDateTime(vm.CheckOut),
                GuestsCount = vm.GuestsCount,
                IsBreakfast = vm.IsBreakfast
            };
        }
    }
}
