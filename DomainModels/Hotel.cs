using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        public string? HotelName { get; set; }
        public required string? CityName { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}