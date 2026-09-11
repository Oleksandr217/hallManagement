using src.Models.DTO;

namespace src.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResultDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default);
        Task<BookingResultDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task CancelAsync(Guid id, CancellationToken ct = default);
    }
}
