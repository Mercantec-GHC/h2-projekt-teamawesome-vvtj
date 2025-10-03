using System.ComponentModel.DataAnnotations;
using BlazorBootstrap;
using Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages;

public partial class Home
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    private ToastService ToastService { get; set; } = null!;


    private DateOnly? _startDate;
    private DateOnly? _endDate;
    private bool _disableEndDate = true;
    private string? _selectedHotel;
    private int _amountGuests;


    private List<string> Hotels { get; set; } = new();
    [Inject]
    private APIService APIService { get; set; } = null!;
   
    protected override async Task OnInitializedAsync()
    {

        var hotels = await APIService.GetAllHotelsAsync();
        if (hotels != null)
        {
            Hotels = hotels.Select(h => h.HotelName)
                           .Where(n => !string.IsNullOrWhiteSpace(n))
                           .Distinct()
                           .OrderBy(n => n)
                           .ToList();
        }
        else
        {
            Hotels = new List<string>();
        }

        _amountGuests = 1;

    }

    private void NavigateToBooking()
    {
        if (_startDate.HasValue && _endDate.HasValue &&
            !string.IsNullOrWhiteSpace(_selectedHotel) &&
            _amountGuests >= 1)
        {
            var url = $"/booking" +
                      $"?hotel={Uri.EscapeDataString(_selectedHotel)}" +
                      $"&guests={_amountGuests}" +
                      $"&checkIn={_startDate:yyyy-MM-dd}" +
                      $"&checkOut={_endDate:yyyy-MM-dd}";



            NavigationManager.NavigateTo(url);
        }
        else
        {
            ToastService.Notify(new ToastMessage(
                ToastType.Warning,
                iconName: IconName.ExclamationTriangle,
                title: "Missing information",
                message: "Please fill in all fields."
            ));
        }
    }

    private void StartDateChanged(DateOnly? startDate)
    {
        if (!startDate.HasValue || startDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            _startDate = null;
            _endDate = null;
            _disableEndDate = true;
            return;
        }

        _startDate = startDate;
        _endDate = null;
        _disableEndDate = false;
    }

    private DateOnly? GetMinEndDate()
        => _startDate.HasValue ? _startDate.Value.AddDays(1) : null;

    public class Booking
    {
        [Required] public DateOnly? StartDate { get; set; }
        [Required] public DateOnly? EndDate { get; set; }
        [Required] public string? Hotel { get; set; }
        [Range(1, 6)] public int Guests { get; set; } = 1;
    }
 
    private void NavigateToRooms()
    {
        if (_startDate.HasValue && _endDate.HasValue && !string.IsNullOrEmpty(_selectedHotel))
        {
            var url = $"/rooms?start={_startDate:yyyy-MM-dd}&end={_endDate:yyyy-MM-dd}&hotel={_selectedHotel}&guests={_amountGuests}";
            NavigationManager.NavigateTo(url);
        }
        else
        {
            ToastService.Notify(new ToastMessage(
                ToastType.Warning,
                iconName: IconName.ExclamationTriangle,
                title: "Missing Information",
                message: "Please fill in all fields."
            ));
        }
    }


}