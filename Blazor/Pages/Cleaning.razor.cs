using Blazor.Models;
using Blazor.Models.ViewModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages
{
	public partial class Cleaning
	{
		public List<CleaningViewModel> CleaningViewModel { get; set; } = new ();
		public IEnumerable<HotelDto> HotelDtos { get; set; } = new List<HotelDto>();

		[SupplyParameterFromForm(FormName = "CleaningFormModel")]
		CleaningForm CleaningFormModel { get; set; } = new();

		public string? ErrorMarkedRooms;
		public string? ErrorLoadData { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await LoadRooms();
		}

		private async Task LoadRooms()
		{
			var roomsToCleaning = (await ApiService.GetAllRoomsToClean())?.ToList();
			if (roomsToCleaning != null)
			{
				HotelDtos = await ApiService.GetAllHotelsAsync() ?? new List<HotelDto>();
				CleaningViewModel = roomsToCleaning.Select(r =>
				{
					var hotelName = HotelDtos.FirstOrDefault(h => h.Id == r.HotelId)?.HotelName ?? "Unknown Hotel";

					return new CleaningViewModel
					{
						Hotel = hotelName,
						RoomNumbers = r.RoomNumbers.ToList()
					};
				}).ToList();
			}
			else
			{
				ErrorLoadData = "Something went wrong. Rooms to cleaning can not be loaded, contact support.";
			}
		}

		private async Task HandleSubmit()
		{
			var roomNumbers = CleaningFormModel.RoomNumbersInput
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(r => int.Parse(r.Trim()))
				.ToList();

			var roomsToCleanList = new List<RoomToCleanDto>
			{
				new RoomToCleanDto
				{
					HotelId = CleaningFormModel.SelectedHotelId,
					RoomNumbers = roomNumbers
				}
			};

			var success = await ApiService.MarkRoomsAsCleanedAsync(roomsToCleanList);
			if (success)
			{
				CleaningFormModel.RoomNumbersInput = string.Empty;
				await LoadRooms();
			}
			else
			{
				ErrorMarkedRooms = "Something went wrong. Rooms were not marked as cleaned, contact support.";
			}
		}
	}
}
