using Blazor.Models.ViewModels;
using Blazor.Services;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages
{
    public class AvailableRoomsBase : ComponentBase
    {
        [Inject] protected APIService Api { get; set; } = default!;

        protected AvailableRoomsViewModel Vm { get; } = new();
        protected List<HotelDto> Hotels { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            // default dates
            Vm.From = DateOnly.FromDateTime(DateTime.Today);
            Vm.To = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            Hotels = await Api.GetHotelsAsync();

        }

        protected async Task SearchAsync()
        {
            Vm.Error = null;
            Vm.IsLoading = true;

            try
            {
                
                if (string.IsNullOrWhiteSpace(Vm.HotelName))
                {
                    Vm.Error = "Please specify a hotel name.";
                    return;
                }
                if (Vm.To <= Vm.From)
                {
                    Vm.Error = "'To' date must be after 'From' date.";
                    return;
                }

                var rooms = await Api.GetAvailableRoomsAsync(Vm.HotelName, Vm.From, Vm.To);
                Vm.Rooms = rooms;
            }
            catch (Exception ex)
            {
                Vm.Error = "Failed to load available rooms.";
                Console.WriteLine(ex);
            }
            finally
            {
                Vm.IsLoading = false;
            }
        }
    }
}
