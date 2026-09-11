namespace src.Models.DTO
{
    public record HallOccupancyReportDto(
    Guid HallId,
    string HallName,
    int TotalBookings,
    double TotalHoursBooked,
    double OccupancyRatePercent,
    decimal Revenue);

    /// <summary>
    /// The company's revenue for the period, grouped by day.
    /// </summary>
    public record RevenueReportItemDto(DateOnly Date, int BookingsCount, decimal Revenue);

    /// <summary>
    /// Popularity of services: how many times they were selected and how much revenue they generated.
    /// </summary>
    public record PopularAmenityDto(Guid AmenityId, string AmenityName, int TimesBooked, decimal TotalRevenue);

    /// <summary>
    /// Average reservation amount for the period.
    /// </summary>
    public record AverageBookingValueDto(decimal AverageValue, int BookingsCount);
}
