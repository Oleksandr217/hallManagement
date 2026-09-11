using Microsoft.EntityFrameworkCore;
using src.Interfaces;
using src.Models.Domain;
using src.Models.Enum;

namespace src.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context) => _context = context;

        public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _context.Bookings
                .Include(b => b.Hall)
                .Include(b => b.BookingAmenities).ThenInclude(bs => bs.Amenity)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<List<Booking>> GetAllAsync(CancellationToken ct = default) =>
            _context.Bookings
                .Include(b => b.Hall)
                .Include(b => b.BookingAmenities).ThenInclude(bs => bs.Amenity)
                .OrderByDescending(b => b.StartTime)
                .IgnoreQueryFilters()
                .ToListAsync(ct);

        public Task<List<Booking>> GetOverlappingBookingsAsync(
            Guid hallId, DateTime start, DateTime end, CancellationToken ct = default) =>
            _context.Bookings
                .Where(b => b.HallId == hallId
                            && b.Status == BookingStatus.Confirmed
                            && b.StartTime < end
                            && b.EndTime > start)
                .ToListAsync(ct);

        public Task<List<Booking>> GetActiveBookingsForHallsAsync(
            IEnumerable<Guid> hallIds, DateTime rangeStart, DateTime rangeEnd, CancellationToken ct = default) =>
            _context.Bookings
                .Where(b => hallIds.Contains(b.HallId)
                            && b.Status == BookingStatus.Confirmed
                            && b.StartTime < rangeEnd
                            && b.EndTime > rangeStart)
                .ToListAsync(ct);

        public Task<List<Booking>> GetInPeriodAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
            _context.Bookings
                .Include(b => b.BookingAmenities).ThenInclude(bs => bs.Amenity)
                .Where(b => b.StartTime >= from && b.StartTime < to)
                .IgnoreQueryFilters()
                .ToListAsync(ct);

        public async Task AddAsync(Booking booking, CancellationToken ct = default) =>
            await _context.Bookings.AddAsync(booking, ct);

        public void Update(Booking booking) => _context.Bookings.Update(booking);

        public Task SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }   
}
