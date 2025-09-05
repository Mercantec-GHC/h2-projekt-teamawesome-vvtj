namespace DomainModels.Dto
{
    public class RoomToCleanDto
    {
        public int HotelId { get; set; }
        public IEnumerable<int> RoomNumbers { get; set; }
    }
}
