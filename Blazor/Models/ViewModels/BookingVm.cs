using DomainModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Models.ViewModels
{
    public class BookingVm
    {
        public string FullName { get; set; } = string.Empty;

        [Required] public string UserName { get; set; } = string.Empty;
        [Required] public string HotelName { get; set; } = string.Empty;
        [Required] public RoomTypeEnum RoomType { get; set; }
        [Required] public DateTime CheckIn { get; set; }
        [Required] public DateTime CheckOut { get; set; }

        [Range(1, 10)] public int GuestsCount { get; set; } = 1;

        public int NightsCount { get; set; }
        public bool IsBreakfast { get; set; }
    }
}
