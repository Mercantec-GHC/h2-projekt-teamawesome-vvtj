using Blazor.Models;
using Blazor.Models.ViewModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;
using DomainModels.Models;
using Blazor.Services;
using System.Threading.Tasks;
using System.Linq;

namespace Blazor.Pages
{
    public partial class Rooms
    {
        [Inject]
        public APIService _aPIService { get; set; } = default!;
        
        public IEnumerable<RoomTypeDto> roomTypes { get; set; } = new List<RoomTypeDto>();

        protected override async Task OnInitializedAsync()
        {
            System.Console.WriteLine("OnInitializedAsync: Getting room types");
            var result = await _aPIService.GetRoomTypeAsync()
                ?? throw new ArgumentException("Error: No Room types were found");
            roomTypes = result?.Where(r => r != null)!;
        }
    }
}