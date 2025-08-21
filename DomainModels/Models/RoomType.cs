using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models
{
    public class RoomType : Common
    {
        
        public required string TypeofRoom { get; set; }
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal? PricePerNight { get; set; }
    }
}