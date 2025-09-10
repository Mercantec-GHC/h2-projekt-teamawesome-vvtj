using DomainModels.Dto;
using DomainModels.Enums;

namespace API.Services
{
    public class SeasonalPricingService
    {
        enum Seasons
        {
            Summer,
            Autumn,
            Winter,
            Spring,
        }
        
        /// <summary>
        /// Dictionary to show markup percentage
        /// </summary>
        private readonly Dictionary<Seasons, decimal> _seasonalMarkup = new()
        {
            {Seasons.Summer, 0.20m},
            {Seasons.Autumn, 0.00m},
            {Seasons.Winter, 0.10m},
            {Seasons.Spring, 0.00m}

        };

        /// <summary>
        /// A switch expression method for the different months
        /// </summary>
        /// <param name="bookingDate">Date which the guests wants to stay in</param>
        /// <returns>Returns corresponding season based on month which the dates are in</returns>
        private Seasons GetSeason(DateTime bookingDate) => bookingDate.Month switch
        {
            //June - August
            >= 6 and <= 8 => Seasons.Summer,
            //December - February
            12 or 1 or 2 => Seasons.Winter,
            //March - May
            >= 3 and <= 5 => Seasons.Spring,
            //Every other month - Autum
            _ => Seasons.Autumn
        };


        /// <summary>
        /// Method for seasonal pricing of a booking
        /// </summary>
        /// <param name="Price">The base price for a room (From the database)</param>
        /// <param name="bookingDate">The date the guest wants to stay</param>
        /// <returns>The price with the seasonal markup. </returns>
        public async Task<decimal> GetSeasonalPrice(decimal Price, DateTime bookingDate)
        {
            var season = GetSeason(bookingDate);
            var markup = _seasonalMarkup[season];

            //
            return Price * (1 + markup);
        }
    }
}