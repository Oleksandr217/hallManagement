namespace src.Models.DTO
{
    public class CreateBookingDto
    {
        public Guid HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Guid> AmenityIds { get; set; } = new();
    }

    /// <summary>
    ///  A single rate segment in the cost calculation.
    /// </summary>
    public record PricingSegmentDto(
        DateTime From,
        DateTime To,
        string PeriodName,
        decimal HourlyRateApplied,
        decimal Cost);

    /// <summary>
    /// Complete booking details, including a breakdown of the cost.
    /// </summary>
    public record BookingResultDto(
        Guid Id,
        Guid HallId,
        string HallName,
        DateTime StartTime,
        DateTime EndTime,
        IReadOnlyList<PricingSegmentDto> PricingBreakdown,
        decimal HallRentCost,
        decimal AmenitysCost,
        decimal TotalCost,
        IReadOnlyList<AmenityDto> Amenitys);

}
