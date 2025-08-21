public class RoomTypeDto
{
    public int Id { get; set; }
    public required string TypeofRoom { get; set; }
    public int MaxCapacity { get; set; }
    public string? Description { get; set; }

    //rules
    public bool? HasKitchenette { get; set; }
    public bool? HasTowels { get; set; }
    public bool? HasBalcony { get; set; }
    public bool? HasJacuzzi { get; set; }
}