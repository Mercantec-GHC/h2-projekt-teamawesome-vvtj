using Blazor.Models;
using Blazor.Models.ViewModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;
using DomainModels.Models;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.JSInterop;

namespace Blazor.Pages
{
    
    public partial class Rooms
    {
        [Inject]
        protected PreloadService PreloadService { get; set; } = default!;
        [Inject]
        public APIService _aPIService { get; set; } = default!;

        public IEnumerable<RoomTypeDto> roomTypes { get; set; } = new List<RoomTypeDto>();

        protected override async Task OnInitializedAsync()
        {
            PreloadService.Show();

            System.Console.WriteLine("OnInitializedAsync: Getting room types");
            var result = await _aPIService.GetRoomTypeAsync()
                ?? throw new ArgumentException("Error: No Room types were found");
            roomTypes = result?.Where(r => r != null)!;

            PreloadService.Hide();
        }
    }
}