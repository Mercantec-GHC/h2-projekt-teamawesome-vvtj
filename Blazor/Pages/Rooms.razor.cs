
using Microsoft.AspNetCore.Components;
using Blazor.Services;

namespace Blazor.Pages
{
    
    public partial class Rooms
    {
        [Inject]
        public APIService _aPIService { get; set; } = default!;
        public IEnumerable<RoomTypeDto> roomTypes { get; set; } = new List<RoomTypeDto>();
        public bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {

            IsLoading = true;
            System.Console.WriteLine("OnInitializedAsync: Getting room types");
            var result = await _aPIService.GetRoomTypeAsync()
                ?? throw new ArgumentException("Error: No Room types were found");
            roomTypes = result?.Where(r => r != null)!;

            IsLoading = false;
        }
    }
}