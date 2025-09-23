using Microsoft.AspNetCore.Components;
using Blazor.Services;



namespace Blazor
{
    

    public class Rooms
    {
        public override async Task OnInitializedAsync()
        {

        }

        private async Task ShowRoomTypes()
        {
            var roomTypes = (await ApiService.())?.ToList();
        }
    }
}