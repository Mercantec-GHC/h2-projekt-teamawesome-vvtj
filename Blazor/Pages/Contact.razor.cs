using Blazor.Models.ViewModel;
using DomainModels.Dto;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages
{
    public partial class Contact
    {
        public List<HotelDto> Hotels { get; set; } = new();
        [SupplyParameterFromForm(FormName = "ContactForm")]
        ContactViewModel contactForm { get; set; } = new();

        bool isSent = false;
        bool isError = false;
        protected override async Task OnInitializedAsync()
        {
            Hotels = (await ApiService.GetAllHotelsAsync())?.ToList() ?? new List<HotelDto>();
        }

        private async Task HandleSubmit()
        {
            isSent = false;
            isError = false;

            try
            {
                var contactDto = new EmailFormDto
                {
                    Name = contactForm.Name,
                    Email = contactForm.Email,
                    Message = contactForm.Message
                };

                var request = await ApiService.SendEmailAsync(contactDto);

                if (request)
                {
                    isSent = true;
                }
                else
                {
                    isError = true;
                }
            }
            catch (Exception)
            {
                isError = true;
            }
        }
    }
}
