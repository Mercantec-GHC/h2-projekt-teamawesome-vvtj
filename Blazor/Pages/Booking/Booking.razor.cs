using Blazor.Models.ViewModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;


namespace Blazor.Pages.Booking;

public partial class Booking : ComponentBase
{
	[Inject] private Services.APIService Api { get; set; } = default!;
	[Inject] private NavigationManager Nav { get; set; } = default!;

	protected bool IsSubmitting { get; set; }
	protected string? FormError { get; set; }
	protected CreateBookingViewModel Vm { get; set; } = new();
	protected BookingResponseDto? Quote { get; set; }

	protected CurrentUserProfileViewModel? User { get; set; }
	protected List<string> Hotels { get; set; } = new();
	protected List<RoomTypeDto> AllowedRoomTypes { get; set; } = new();

	protected bool IsRoomTypeEnabled => Vm is not null && Vm.GuestsCount >= 1;
	protected bool IsSuccess { get; set; } = false;
	protected bool CanProceed =>
		Vm is not null &&
		!string.IsNullOrWhiteSpace(Vm.HotelName) &&
		Vm.GuestsCount >= 1 &&
		 Vm.GuestsCount <= 6 &&
		Vm.NightsCount > 0 &&
		Vm.CheckIn >= DateTime.Today;

	protected override async Task OnInitializedAsync()
	{

		var user = await Api.GetCurrentUserAsync();

		var userInfo = await Api.GetCurrentUserInfoAsync();
		if (userInfo != null)
		{
			User = new CurrentUserProfileViewModel
			{
				FirstName = userInfo.FirstName ?? string.Empty,
				LastName = userInfo.LastName ?? string.Empty
			};
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

        if (int.TryParse(q.Get("roomTypeId"), out var Id))
			Vm.RoomTypeId = Id;
        var hotel = q.Get("hotel");
        if (!string.IsNullOrWhiteSpace(hotel))
            Vm.HotelName = hotel;

		if (int.TryParse(q.Get("guests"), out var guests) && guests > 0)
			Vm.GuestsCount = guests;

		if (int.TryParse(q.Get("roomTypeId"), out var Id))
			Vm.RoomTypeId = Id;
		var hotel = q.Get("hotel");
		if (!string.IsNullOrWhiteSpace(hotel))
			Vm.HotelName = hotel;

		if (DateTime.TryParse(q.Get("checkIn"), out var ci))
			Vm.CheckIn = ci.Date;

		if (DateTime.TryParse(q.Get("checkOut"), out var co))
			Vm.CheckOut = co.Date;



		RecalcNights();
	}
	private void CancelCreate()
	{
		Nav.NavigateTo("/");
	}

	protected void OnHotelChanged(ChangeEventArgs _) => ClearError();
	protected void OnRoomTypeChanged(ChangeEventArgs _) => ClearError();
	protected void OnGuestsChanged(ChangeEventArgs _)
	{
		if (Vm.GuestsCount > 6)
		{
			Vm.GuestsCount = 6;
			FormError = "Maximum 6 guests in one room";
		}
		else if (Vm.GuestsCount < 1)
		{
			Vm.GuestsCount = 1;
			FormError = "At least 1 guest is required.";
		}
		else
		{
			ClearError();
		}
	}

	protected void OnDatesChanged(ChangeEventArgs _)
	{
		if (Vm.CheckIn < DateTime.Today)
		{
			Vm.CheckIn = DateTime.Today;
			FormError = "Check-in date cannot be earlier than today.";
		}

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
	protected async Task OnPreviewClicked()
	{
		if (!CanProceed || Vm is null) return;

		IsSubmitting = true;
		FormError = null;

		var dto = Vm.ToCreateBookingDto();

		var result = await Api.CreateBooking(dto, preview: true);

		IsSubmitting = false;

		if (result is null)
		{
			FormError = "No rooms of this type are available for the selected dates.";
			Quote = null;
			return;
		}

		Quote = result;
		Vm.TotalPrice = result.TotalPrice;
	}
	protected async Task OnCreateClicked()
	{
		if (!CanProceed || Vm is null) return;

		IsSubmitting = true;
		FormError = null;

		var dto = Vm.ToCreateBookingDto();
		var result = await Api.CreateBooking(dto, preview: false);

		IsSubmitting = false;

		if (result is null)
		{
			FormError = "No rooms of this type are available for the selected dates, or the data is invalid.";
			return;
		}

		Vm.TotalPrice = result.TotalPrice;
		Quote = result;

		IsSuccess = true;


	}
	private void GoToMyBookings()
	{
		Nav.NavigateTo("/user/bookings");
	}
}
