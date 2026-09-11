using src.Interfaces;
using src.Models.Domain;
using src.Models.DTO;
using src.Models.Enum;

namespace src.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IPricingService _pricingService;

        public BookingService(
            IBookingRepository bookingRepository,
            IHallRepository hallRepository,
            IPricingService pricingService)
        {
            _bookingRepository = bookingRepository;
            _hallRepository = hallRepository;
            _pricingService = pricingService;
        }

        public async Task<BookingResultDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default)
        {
            var hall = await _hallRepository.GetByIdAsync(dto.HallId, ct)
                       ?? throw new NotFoundException(nameof(Hall), dto.HallId);

            var pricing = _pricingService.CalculateHallRent(hall, dto.StartTime, dto.EndTime);

            var overlapping = await _bookingRepository.GetOverlappingBookingsAsync(
                hall.Id, dto.StartTime, dto.EndTime, ct);
            if (overlapping.Count > 0)
                throw new ConflictException("Зал вже заброньований на обраний час.");

            var requestedAmenities = await _hallRepository.GetAmenitiesByIdsAsync(dto.AmenityIds, ct);
            var missing = dto.AmenityIds.Except(requestedAmenities.Select(a => a.Id)).ToList();
            if (missing.Count > 0)
                throw new ValidationException($"Послуги не знайдено: {string.Join(", ", missing)}.");

            var availableAmenityIds = hall.HallAmenities.Select(ha => ha.AmenityId).ToHashSet();
            var notOffered = requestedAmenities.Where(a => !availableAmenityIds.Contains(a.Id)).ToList();
            if (notOffered.Count > 0)
                throw new ValidationException(
                    $"Ці послуги недоступні в обраному залі: {string.Join(", ", notOffered.Select(a => a.Name))}.");

            var amenitiesCost = requestedAmenities.Sum(a => a.Price);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                HallId = hall.Id,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalCost = pricing.HallRentCost + amenitiesCost,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                BookingAmenities = requestedAmenities
                    .Select(a => new BookingAmenity { AmenityId = a.Id, PriceAtBooking = a.Price })
                    .ToList()
            };

            await _bookingRepository.AddAsync(booking, ct);
            await _bookingRepository.SaveChangesAsync(ct);

            return BuildResultDto(booking, hall, pricing.Breakdown, requestedAmenities, pricing.HallRentCost, amenitiesCost);
        }

        public async Task<BookingResultDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, ct)
                          ?? throw new NotFoundException(nameof(Booking), id);

            var hall = booking.Hall;

            var pricing = _pricingService.CalculateHallRent(hall, booking.StartTime, booking.EndTime);
            var amenities = booking.BookingAmenities.Select(ba => ba.Amenity).ToList();
            var amenitiesCost = booking.BookingAmenities.Sum(ba => ba.PriceAtBooking);

            return BuildResultDto(booking, hall, pricing.Breakdown, amenities, pricing.HallRentCost, amenitiesCost);
        }

        public async Task CancelAsync(Guid id, CancellationToken ct = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, ct)
                          ?? throw new NotFoundException(nameof(Booking), id);

            if (booking.Status == BookingStatus.Cancelled)
                throw new ConflictException("Бронювання вже скасовано.");

            booking.Status = BookingStatus.Cancelled;
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync(ct);
        }

        private static BookingResultDto BuildResultDto(
            Booking booking,
            Hall hall,
            IReadOnlyList<PricingSegmentDto> breakdown,
            List<Amenity> amenities,
            decimal hallRentCost,
            decimal amenitiesCost) => new(
                booking.Id,
                hall.Id,
                hall.Name,
                booking.StartTime,
                booking.EndTime,
                breakdown,
                hallRentCost,
                amenitiesCost,
                hallRentCost + amenitiesCost,
                amenities.Select(a => new AmenityDto(a.Id, a.Name, a.Price)).ToList());
    }
}