using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }
        public required string TypeofRoom { get; set; }
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }
        public double? PricePerNight { get; set; }
    }
}