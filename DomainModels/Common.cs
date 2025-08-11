using System.ComponentModel.DataAnnotations;

namespace DomainModels
{
    public class Common
    {
        [Key]
        public int Id { get; set; } // Kan erstattes med "int Id"
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
