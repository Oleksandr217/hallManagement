using src.Models.Domain;

namespace src.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Booking>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Returns active room reservations that overlap with the specified time interval.
        /// Used both to check for conflicts when creating a reservation
        /// and to search for available rooms.
        /// </summary>
        Task<List<Booking>> GetOverlappingBookingsAsync(
            Guid hallId, DateTime start, DateTime end, CancellationToken ct = default);

        /// <summary>Reserved time slots for room selection within a 24-hour period for checking availability.</summary>
        Task<List<Booking>> GetActiveBookingsForHallsAsync(
            IEnumerable<Guid> hallIds, DateTime rangeStart, DateTime rangeEnd, CancellationToken ct = default);

        Task<List<Booking>> GetInPeriodAsync(DateTime from, DateTime to, CancellationToken ct = default);

        Task AddAsync(Booking booking, CancellationToken ct = default);
        void Update(Booking booking);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
