public class RoomTypeDto
    {
        public int Id { get; set; }
        public required string TypeofRoom { get; set; }
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }
    }