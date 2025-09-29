using DomainModels.Enums;

public class RoomTypeDto
{
    public int Id { get; set; }
    public string? TypeofRoom { get; set; }
    public int MaxCapacity { get; set; }
    public string? Description { get; set; }
    public decimal? PricePerNight { get; set; }

    //rules
    public bool? HasKitchenette { get; set; }
    public bool? HasExtraTowels { get; set; }
    public bool? HasBalcony { get; set; }
    public bool? HasJacuzzi { get; set; }
    public bool? HasSeaView { get; set; }
    public bool? HasGardenView { get; set; }
    public bool? HasAirCondition { get; set; }
    public bool? HasTV { get; set; }
    public bool? HasKettle { get; set; }
    public bool? HasMiniFridge { get; set; }
    public int? Area { get; set; }
    public bool? HasVault { get; set; }
    public string? ImagePath{ get; set; }
}

public class ToRoomTypeGETdto
{
    public required RoomTypeEnum TypeofRoom { get; set; }
}