using DomainModels.Dto;

namespace Blazor.Pages
{
    public partial class Contact
    {
        public List<HotelDto> Hotels { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Hotels = (await ApiService.GetAllHotelsAsync())?.ToList() ?? new List<HotelDto>();
        }
    }
}
