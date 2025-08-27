using System.ComponentModel.DataAnnotations;
using BlazorBootstrap;
using DomainModels.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages;

public partial class Home
{
	[Inject] 
	private NavigationManager NavigationManager { get; set; } = null!;
	[Inject]
	private ToastService ToastService { get; set; } = null!;

	private Booking _booking = new();

	private DateOnly? _startDate;
	private DateOnly? _endDate;
	private bool _disableEndDate = true;
	private string? _selectedHotel;
	private int _amountGuests;

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

	private DateOnly? GetMinEndDate() =>
		_startDate.HasValue ? _startDate.Value.AddDays(1) : null;

	public class Booking
	{
		[Required] public DateOnly? StartDate { get; set; }
		[Required] public DateOnly? EndDate { get; set; }
		[Required] public string? Hotel { get; set; }
		[Range(1, 10)] public int Guests { get; set; }
	}
}
