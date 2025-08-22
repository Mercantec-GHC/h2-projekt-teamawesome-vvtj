using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BookingResponseDto
{
    public string UserName { get; set; }
    public string HotelName { get; set; }
    public string RoomType { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestsCount { get; set; }
    public int NightsCount { get; set; }
    public decimal TotalPrice { get; set; }
}