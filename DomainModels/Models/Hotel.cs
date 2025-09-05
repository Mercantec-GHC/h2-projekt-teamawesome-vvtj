namespace DomainModels.Models
{
    public class Hotel : Common
    {
        public required string HotelName { get; set; }
        public required string CityName { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? WeekdayTime { get; set; }
        public string? SaturdayTime { get; set; } 
        public string? HolidaysTime { get; set; }
        public List<Room> Rooms { get; set; } = new(); // 1:n
    }

}