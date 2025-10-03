using Blazor.Models.ViewModel;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages
{
    public partial class AboutUs
    {
        public List<HotelDto> Hotels { get; set; } = new();
        public bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            Hotels = (await ApiService.GetAllHotelsAsync())?.ToList() ?? new List<HotelDto>();
            IsLoading = false;
        }
    }
    
}
