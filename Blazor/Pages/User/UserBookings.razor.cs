namespace Blazor.Pages.User;
using Blazor.Services;
using DomainModels.Dto;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserBookingsBase : ComponentBase
{
    [Inject] protected APIService ApiService { get; set; } = default!;

    protected List<BookingByUserDto>? Bookings { get; set; }
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Bookings = await ApiService.GetBookingsByCurrentUserAsync();
        }
        catch
        {
            Bookings = new List<BookingByUserDto>();
        }
        finally
        {
            IsLoading = false;
        }
    }
    protected async Task DeleteBookingAsync(int bookingId)
    {
        var success = await ApiService.DeleteMyBookingAsync(bookingId);

        if (success)
        {
            Bookings = (Bookings ?? new List<BookingByUserDto>())
                       .Where(b => b.Id != bookingId)
                       .ToList();
            StateHasChanged();
        }
        else
        {
            Console.WriteLine("Failed to delete booking.");
        }
    }
}