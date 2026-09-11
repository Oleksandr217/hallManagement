using src.Models.DTO;

namespace src.Interfaces
{
    public interface IReportService
    {
        Task<List<HallOccupancyReportDto>> GetHallOccupancyReportAsync(
        DateTime from, DateTime to, CancellationToken ct = default);

        Task<List<RevenueReportItemDto>> GetRevenueReportAsync(
            DateTime from, DateTime to, CancellationToken ct = default);

        Task<List<PopularAmenityDto>> GetPopularServicesReportAsync(
            DateTime from, DateTime to, CancellationToken ct = default);

        Task<AverageBookingValueDto> GetAverageBookingValueAsync(
            DateTime from, DateTime to, CancellationToken ct = default);
    }
}
