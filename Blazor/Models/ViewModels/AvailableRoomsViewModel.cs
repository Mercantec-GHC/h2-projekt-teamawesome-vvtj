//namespace Blazor.Models.ViewModels
//{
//    using System.ComponentModel.DataAnnotations;
//    using DomainModels.Dto; 

//    public class AvailableRoomsViewModel
//    {
//        [Required]
//        public string HotelName { get; set; } = string.Empty;

//        [Required]
//        public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.Today);

//        [Required]
//        public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

//        public List<GetAvailableRoomsDto> Rooms { get; set; } = new();

//        public bool IsLoading { get; set; }
//        public string? Error { get; set; }
//    }
//}
