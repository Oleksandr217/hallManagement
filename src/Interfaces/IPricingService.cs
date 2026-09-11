using src.Models.Domain;
using src.Models.DTO;

namespace src.Interfaces
{
    public record PricingResult(
    IReadOnlyList<PricingSegmentDto> Breakdown,
    decimal HallRentCost);

    public interface IPricingService
    {
        /// <summary>
        /// Calculates the cost of renting the hall for the period [start, end), dividing it
        /// into segments according to the rate tiers (morning/standard/peak/evening).
        /// </summary>
        PricingResult CalculateHallRent(Hall hall, DateTime start, DateTime end);
    }
}
