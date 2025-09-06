using DomainModels;
using DomainModels.Dto;

namespace API.Interfaces
{
    public interface ICleaningService
    {
        Task<IEnumerable<RoomToCleanDto>> GetAllRoomsToCleanAsync();
        Task<List<RoomToCleanDto?>> MarkRoomAsCleanedAsync(List<RoomToCleanDto> roomNumbers);
    }
}
