using src.Interfaces;
using src.Models.DTO;
using src.Models.Enum;

namespace src.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHallRepository _hallRepository;

        public ReportService(IBookingRepository bookingRepository, IHallRepository hallRepository)
        {
            _bookingRepository = bookingRepository;
            _hallRepository = hallRepository;
        }

        public async Task<List<HallOccupancyReportDto>> GetHallOccupancyReportAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var halls = await _hallRepository.GetAllAsync(ct);
            var bookings = (await _bookingRepository.GetInPeriodAsync(from, to, ct))
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToList();

            var totalPeriodHours = Math.Max((to - from).TotalHours, 0.0001);

            return halls.Select(hall =>
            {
                var hallBookings = bookings.Where(b => b.HallId == hall.Id).ToList();
                var totalHours = hallBookings.Sum(b => (b.EndTime - b.StartTime).TotalHours);

                return new HallOccupancyReportDto(
                    hall.Id,
                    hall.Name,
                    hallBookings.Count,
                    Math.Round(totalHours, 2),
                    Math.Round(totalHours / totalPeriodHours * 100, 2),
                    hallBookings.Sum(b => b.TotalCost));
            })
            .OrderByDescending(r => r.Revenue)
            .ToList();
        }

        public async Task<List<RevenueReportItemDto>> GetRevenueReportAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var bookings = (await _bookingRepository.GetInPeriodAsync(from, to, ct))
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToList();

            return bookings
                .GroupBy(b => DateOnly.FromDateTime(b.StartTime))
                .Select(g => new RevenueReportItemDto(g.Key, g.Count(), g.Sum(b => b.TotalCost)))
                .OrderBy(r => r.Date)
                .ToList();
        }

        public async Task<List<PopularAmenityDto>> GetPopularServicesReportAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var bookings = (await _bookingRepository.GetInPeriodAsync(from, to, ct))
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToList();

            return bookings
                .SelectMany(b => b.BookingAmenities)
                .GroupBy(bs => new { bs.AmenityId, bs.Amenity.Name })
                .Select(g => new PopularAmenityDto(
                    g.Key.AmenityId,
                    g.Key.Name,
                    g.Count(),
                    g.Sum(bs => bs.PriceAtBooking)))
                .OrderByDescending(r => r.TimesBooked)
                .ToList();
        }

        public async Task<AverageBookingValueDto> GetAverageBookingValueAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var bookings = (await _bookingRepository.GetInPeriodAsync(from, to, ct))
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToList();

            if (bookings.Count == 0)
                return new AverageBookingValueDto(0, 0);

            return new AverageBookingValueDto(
                Math.Round(bookings.Average(b => b.TotalCost), 2),
                bookings.Count);
        }
    }
}
