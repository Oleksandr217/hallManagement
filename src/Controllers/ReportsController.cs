using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Models.DTO;

namespace src.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Produces("application/json")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService) => _reportService = reportService;

        [HttpGet("hall-occupancy")]
        public async Task<ActionResult<List<HallOccupancyReportDto>>> HallOccupancy(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        {
            var (start, end) = ResolvePeriod(from, to);
            return Ok(await _reportService.GetHallOccupancyReportAsync(start, end, ct));
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<List<RevenueReportItemDto>>> Revenue(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        {
            var (start, end) = ResolvePeriod(from, to);
            return Ok(await _reportService.GetRevenueReportAsync(start, end, ct));
        }

        [HttpGet("popular-services")]
        public async Task<ActionResult<List<PopularAmenityDto>>> PopularServices(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        {
            var (start, end) = ResolvePeriod(from, to);
            return Ok(await _reportService.GetPopularServicesReportAsync(start, end, ct));
        }

        [HttpGet("average-booking-value")]
        public async Task<ActionResult<AverageBookingValueDto>> AverageBookingValue(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        {
            var (start, end) = ResolvePeriod(from, to);
            return Ok(await _reportService.GetAverageBookingValueAsync(start, end, ct));
        }

        private static (DateTime From, DateTime To) ResolvePeriod(DateTime? from, DateTime? to)
        {
            var end = to ?? DateTime.UtcNow;
            var start = from ?? end.AddDays(-30);
            return (start, end);
        }
    }
}
