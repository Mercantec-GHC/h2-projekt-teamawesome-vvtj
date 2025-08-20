using System.ComponentModel.DataAnnotations;

namespace DomainModels.Dto
{
    public class HotelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel name is required")]
		[Display(Name = "Hotel name")]
		public required string HotelName { get; set; }

        [Required(ErrorMessage = "City name is required")]
        public required string CityName { get; set; }

        [Required(ErrorMessage = "Address is required")]
		public required string Address { get; set; }
        public string? Description { get; set; }
    }

    // //Get - Read
    // public class HotelViewDto
    // {
    //     public int Id { get; set; }
    //     public string HotelName { get; set; }
    //     public string CityName { get; set; }

    //     public string? Description { get; set; }
    //     //public string Address { get; set; } ??

    // }

    // //Post - Create
    // public class HotelCreateDto
    // {
    //     public string HotelName { get; set; }
    //     public string CityName { get; set; }
    //     public string Address { get; set; }
    //     public string? Description { get; set; }
    // }

    // //Put - Update
    // public class HotelUpdateDto
    // {
    //     public string HotelName { get; set; }
    //     public string CityName { get; set; }
    //     public string Address { get; set; }
    //     public string? Description { get; set; }
    // }
}