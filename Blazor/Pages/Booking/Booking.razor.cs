//using DomainModels.Dto;
//using DomainModels.Enums;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Components;
//using Blazor.Models.ViewModels;
//namespace Blazor.Pages;

//[Authorize]
//public partial class Booking : ComponentBase
//{
//    [Inject] private Blazor.Services.APIService Api { get; set; } = default!;
//    [Inject] private NavigationManager Nav { get; set; } = default!;

//    // UI state
//   protected bool IsSubmitting { get; set; }
//    protected bool ShowReview { get; set; }
//    protected string? FormError { get; set; }

//    // Data
//    protected BookingVm? Vm { get; set; }
//    protected BookingResponseDto? Quote { get; set; }

//    // hotel + roomType 
//    protected List<string> Hotels { get; set; } = new();
//    protected bool IsRoomTypeEnabled => Vm is not null && Vm.GuestsCount >= 1;
//    protected List<RoomTypeEnum> AllowedRoomTypes { get; set; } = Enum.GetValues<RoomTypeEnum>().ToList();

//    // availability cache (optional; depends on API)
//    protected HashSet<DateOnly> UnavailableDates { get; set; } = new();

//    protected bool CanBook =>
//        Vm is not null &&
//        !string.IsNullOrWhiteSpace(Vm.HotelName) &&
//        Vm.RoomType != default &&
//        Vm.GuestsCount >= 1 &&
//        Vm.NightsCount > 0 &&
//        IsSelectionAvailable();

//    protected override async Task OnInitializedAsync()
//    {
//            // current user
//            var user = await Api.GetCurrentUserAsync();
//            if (user is null)
//            {
//                Nav.NavigateTo("authentication/login");
//                return;
//            }
                       
//            Hotels = (await Api.GetAllHotelsAsync())?.Select(h => h.HotelName).ToList() ?? new List<string>();

//            Vm = new BookingVm
//            {
//              //  FullName = $"{user.FirstName} {user.LastName}".Trim(),
//                UserName = user.UserName,
//                GuestsCount = 1,
//                RoomType = 0, // not selected
//                HotelName = "",
//                CheckIn = DateTime.Today.AddDays(1),
//                CheckOut = DateTime.Today.AddDays(2),
//                IsBreakfast = false
//            };

//            // prefill from query string if present
//            PrefillFromQuery();

//            RecalcNights();

//            // initial availability fetch 
//            if (!string.IsNullOrWhiteSpace(Vm.HotelName) && Vm.RoomType != 0)
//                await LoadAvailabilityForMonth(Vm.CheckIn.Year, Vm.CheckIn.Month);
         
//    }

//    private void PrefillFromQuery()
//    {
//        var uri = Nav.ToAbsoluteUri(Nav.Uri);
//        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

//        if (Vm is null) return;

//        if (int.TryParse(query.Get("guests"), out var guests) && guests > 0)
//            Vm.GuestsCount = guests;

//        if (Enum.TryParse<RoomTypeEnum>(query.Get("roomType"), true, out var rt))
//            Vm.RoomType = rt;

//        var hotel = query.Get("hotel");
//        if (!string.IsNullOrWhiteSpace(hotel))
//            Vm.HotelName = hotel;

//        if (DateTime.TryParse(query.Get("checkIn"), out var ci))
//            Vm.CheckIn = ci.Date;
//        if (DateTime.TryParse(query.Get("checkOut"), out var co))
//            Vm.CheckOut = co.Date;
//    }

//    protected void OnGuestsChanged(ChangeEventArgs _)
//    {
       
//        StateHasChanged(); // filter room types by capacity 
//    }

//    protected async Task OnRoomTypeChanged(ChangeEventArgs _)
//    {
//        if (Vm is null || string.IsNullOrWhiteSpace(Vm.HotelName) || Vm.RoomType == 0) return;
//        await LoadAvailabilityForMonth(Vm.CheckIn.Year, Vm.CheckIn.Month);
//    }

//    protected async Task OnHotelChanged(ChangeEventArgs _)
//    {
//        if (Vm is null || Vm.RoomType == 0 || string.IsNullOrWhiteSpace(Vm.HotelName)) return;
//        await LoadAvailabilityForMonth(Vm.CheckIn.Year, Vm.CheckIn.Month);
//    }

//    protected void OnDatesChanged(ChangeEventArgs _)
//    {
//        RecalcNights();
//        StateHasChanged();
//    }

//    private void RecalcNights()
//    {
//        if (Vm is null) return;
//        var nights = (Vm.CheckOut.Date - Vm.CheckIn.Date).Days;
//        Vm.NightsCount = Math.Max(0, nights);
//        if (!IsSelectionAvailable())
//            FormError = "Selected dates are not available for the chosen room type.";
//        else
//            FormError = null;
//    }

//    private bool IsSelectionAvailable()
//    {
//        if (Vm is null) return false;
//        if (UnavailableDates.Count == 0) return true; 
//        var range = Enumerable.Range(0, Vm.NightsCount)
//                              .Select(offset => DateOnly.FromDateTime(Vm.CheckIn.AddDays(offset)));
//        return !range.Any(d => UnavailableDates.Contains(d));
//    }

//    private async Task LoadAvailabilityForMonth(int year, int month)
//    {
//        if (Vm is null) return;
//        if (string.IsNullOrWhiteSpace(Vm.HotelName) || Vm.RoomType == 0) return;

//        var from = new DateOnly(year, month, 1);
//        var to = from.AddMonths(1).AddDays(-1);

//    }

//    //protected async Task OnBookClicked()
//    //{
//    //    if (Vm is null) return;

//    //    if (!CanBook)
//    //    {
//    //        FormError = "Please complete the form and choose available dates.";
//    //        return;
//    //    }

//    //    FormError = null;
//    //    IsSubmitting = true;

//    //    var dto = ToCreateBookingDto(Vm, isLocked: false);  // попередній розрахунок, без запису в БД
//    //    Quote = await Api.QuoteBookingAsync(dto);

//    //    IsSubmitting = false;

//    //    if (Quote is null)
//    //    {
//    //        FormError = "Unable to prepare a quote for the selected dates.";
//    //        return;
//    //    }

//    //    // бек вже порахував Nights/TotalPrice — оновимо відображення, якщо треба
//    //    Vm.NightsCount = Quote.NightsCount;
//    //    ShowReview = true;
//    //}

//    protected void BackToEdit()
//    {
//        ShowReview = false;
//        FormError = null;
//    }

//    protected async Task OnPayClicked()
//    {
//        if (Vm is null) return;

//        IsSubmitting = true;
//        FormError = null;

//        var dto = ToCreateBookingDto(Vm, isLocked: true);
//        var result = await Api.CreateBookingAsync(dto);

//        IsSubmitting = false;

//        if (result is null)
//        {
//            FormError = "Payment failed (demo) or booking could not be created.";
//            return;
//        }
//        Nav.NavigateTo($"/booking/success");
//    }

//    private static CreateBookingDto ToCreateBookingDto(BookingVm vm, bool isLocked)
//        => new()
//        {
//            UserName = vm.UserName,
//            TypeOfRoom = vm.RoomType,
//            HotelName = vm.HotelName,
//            CheckIn = DateOnly.FromDateTime(vm.CheckIn),
//            CheckOut = DateOnly.FromDateTime(vm.CheckOut),
//            GuestsCount = vm.GuestsCount,
//            isBreakfast = vm.IsBreakfast,
          
//        };
       
//}
