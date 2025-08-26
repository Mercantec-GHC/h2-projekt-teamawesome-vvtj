using DomainModels.Enums;

namespace API.Helpers
{
    public static class RoomTypeEnumHelper
    {
        public static bool TryToConvert(string input, out RoomTypeEnum result)
        {
            result = default;

            if (Enum.TryParse<RoomTypeEnum>(input, true, out var parsed) &&
               Enum.IsDefined(typeof(RoomTypeEnum), parsed))
            {
                result = parsed;
                return true;
            }
            return false;
        }
    }
}