namespace DomainModels.Dto
{
    //Get - Read
    public class HotelViewDto
    {
        public int Id { get; set; }
        public string HotelName { get; set; }
        public string CityName { get; set; }

        public string? Description { get; set; }
        //public string Address { get; set; } ??

    }

    //Post - Create
    public class HotelCreateDto
    {
        public string HotelName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
    }

    //Put - Update
    public class HotelUpdateDto
    {
        public string HotelName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
    }
}