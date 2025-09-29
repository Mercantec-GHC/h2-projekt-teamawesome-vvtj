using System.ComponentModel.DataAnnotations;
using DomainModels.Dto;
using DomainModels.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Blazor.Models.ViewModels;

namespace Blazor.Pages.Booking;

public partial class Booking : ComponentBase
{
    [Inject] private Services.APIService Api { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    protected bool IsSubmitting { get; set; }
    protected string? FormError { get; set; }
    protected CreateBookingViewModel Vm { get; set; } = new();
    protected BookingResponseDto? Quote { get; set; }

    protected List<string> Hotels { get; set; } = new();
    protected List<RoomTypeDto> AllowedRoomTypes { get; set; } = new();

    protected bool IsRoomTypeEnabled => Vm is not null && Vm.GuestsCount >= 1;

    protected bool CanProceed =>
        Vm is not null &&
        !string.IsNullOrWhiteSpace(Vm.HotelName) &&
       // Vm.RoomTypeId is not null &&
        Vm.GuestsCount >= 1 &&
        Vm.NightsCount > 0;

    protected override async Task OnInitializedAsync()
    {
        var user = await Api.GetCurrentUserAsync();
        if (user is null)
        {
            Nav.NavigateTo("/login");
            return;
        }

        Hotels = (await Api.GetAllHotelsAsync())?.Select(h => h.HotelName).ToList()
                 ?? new List<string>();

        AllowedRoomTypes = await Api.GetAllRoomTypesAsync();

        Vm = new CreateBookingViewModel
        {
            UserName = user.UserName,
            HotelName = string.Empty,
            RoomTypeId = 0,
            GuestsCount = 1,
            CheckIn = DateTime.Today.AddDays(1),
            CheckOut = DateTime.Today.AddDays(2),
            IsBreakfast = false,
            NightsCount = 1
        };

        RecalcNights();
        PrefillFromQuery();
    }

    public void PrefillFromQuery()
    {
        if (Vm is null) return;

        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        var q = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (int.TryParse(q.Get("guests"), out var guests) && guests > 0)
            Vm.GuestsCount = guests;

        //if (Enum.TryParse<RoomTypeEnum>(q.Get("roomType"), true, out var rt))
        //    Vm.RoomTypeId = (int)rt;
        if (int.TryParse(q.Get("roomType"), out var rtId))
            Vm.RoomTypeId = rtId;

        var hotel = q.Get("hotel");
        if (!string.IsNullOrWhiteSpace(hotel))
            Vm.HotelName = hotel;

        if (DateTime.TryParse(q.Get("checkIn"), out var ci))
            Vm.CheckIn = ci.Date;

        if (DateTime.TryParse(q.Get("checkOut"), out var co))
            Vm.CheckOut = co.Date;

        RecalcNights();
    }

    protected void OnHotelChanged(ChangeEventArgs _) => ClearError();
    protected void OnRoomTypeChanged(ChangeEventArgs _) => ClearError();
    protected void OnGuestsChanged(ChangeEventArgs _) => ClearError();

    protected void OnDatesChanged(ChangeEventArgs _)
    {
        RecalcNights();
        ClearError();
    }

    private void RecalcNights()
    {
        if (Vm is null) return;
        var nights = (Vm.CheckOut.Date - Vm.CheckIn.Date).Days;
        Vm.NightsCount = Math.Max(0, nights);

        FormError = Vm.NightsCount <= 0
            ? "Check-out must be after check-in."
            : null;
    }

    private void ClearError() => FormError = null;

    protected async Task OnCreateClicked()
    {
        if (!CanProceed || Vm is null) return;

        IsSubmitting = true;
        FormError = null;

        var dto = Vm.ToCreateBookingDto();
        var result = await Api.CreateBooking(dto);

        IsSubmitting = false;

        if (result is null)
        {
            FormError = "No rooms of this type are available for the selected dates, or the data is invalid.";
            return;
        }

        Vm.TotalPrice = result.TotalPrice;
        Quote = result;

        Nav.NavigateTo("/booking/success");
    }

}
