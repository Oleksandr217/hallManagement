using src.Interfaces;
using src.Models.Domain;
using src.Models.DTO;
using HallServiceLink = src.Models.Domain.HallAmenity;

namespace src.Services
{
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;
        private readonly IBookingRepository _bookingRepository;

        public HallService(IHallRepository hallRepository, IBookingRepository bookingRepository)
        {
            _hallRepository = hallRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<HallDto> CreateAsync(CreateHallDto dto, CancellationToken ct = default)
        {
            ValidateHallInput(dto.Name, dto.Capacity, dto.BaseHourlyRate);

            var services = await _hallRepository.GetAmenitiesByIdsAsync(dto.AmenityId, ct);
            EnsureAllServicesFound(dto.AmenityId, services);

            var hall = new Hall
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Capacity = dto.Capacity,
                BaseHourlyRate = dto.BaseHourlyRate,
                CreatedAt = DateTime.UtcNow,
                HallAmenities = services.Select(s => new HallServiceLink { AmenityId = s.Id }).ToList()
            };

            await _hallRepository.AddAsync(hall, ct);
            await _hallRepository.SaveChangesAsync(ct);

            hall.HallAmenities = hall.HallAmenities.Select(hs => new HallServiceLink { Amenity = services.First(s => s.Id == hs.AmenityId) }).ToList();
            return ToDto(hall);
        }

        public async Task<HallDto> UpdateAsync(Guid id, UpdateHallDto dto, CancellationToken ct = default)
        {
            var hall = await _hallRepository.GetByIdAsync(id, ct)
                        ?? throw new NotFoundException(nameof(Hall), id);

            if (dto.Name is not null)
                hall.Name = dto.Name.Trim();

            if (dto.Capacity is not null)
                hall.Capacity = dto.Capacity.Value;

            if (dto.BaseHourlyRate is not null)
                hall.BaseHourlyRate = dto.BaseHourlyRate.Value;

            ValidateHallInput(hall.Name, hall.Capacity, hall.BaseHourlyRate);

            if (dto.AmenityIds is not null)
            {
                var services = await _hallRepository.GetAmenitiesByIdsAsync(dto.AmenityIds, ct);
                EnsureAllServicesFound(dto.AmenityIds, services);

                hall.HallAmenities.Clear();
                foreach (var s in services)
                    hall.HallAmenities.Add(new HallServiceLink { HallId = hall.Id, AmenityId = s.Id, Amenity = s });
            }

            hall.UpdatedAt = DateTime.UtcNow;
            _hallRepository.Update(hall);
            await _hallRepository.SaveChangesAsync(ct);

            return ToDto(hall);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var hall = await _hallRepository.GetByIdAsync(id, ct)
                        ?? throw new NotFoundException(nameof(Hall), id);

            var now = DateTime.UtcNow;
            var hasFutureBookings = (await _bookingRepository.GetOverlappingBookingsAsync(id, now, now.AddYears(50), ct)).Count > 0;
            if (hasFutureBookings)
                throw new ConflictException("You cannot delete a room that has active upcoming reservations.");

            _hallRepository.Delete(hall);
            await _hallRepository.SaveChangesAsync(ct);
        }

        public async Task<HallDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var hall = await _hallRepository.GetByIdAsync(id, ct)
                        ?? throw new NotFoundException(nameof(Hall), id);

            return ToDto(hall);
        }

        public async Task<List<HallDto>> GetAllAsync(CancellationToken ct = default)
        {
            var halls = await _hallRepository.GetAllAsync(ct);
            return halls.Select(ToDto).ToList();
        }

        public async Task<List<HallDto>> SearchAvailableAsync(HallAvailabilitySearchDto searchDto, CancellationToken ct = default)
        {
            if (searchDto.EndTime <= searchDto.StartTime)
                throw new ValidationException("The end time of the search must be later than the start time.");

            var rangeStart = searchDto.Date.ToDateTime(searchDto.StartTime);
            var rangeEnd = searchDto.Date.ToDateTime(searchDto.EndTime);

            var candidates = await _hallRepository.GetByMinCapacityAsync(searchDto.MinCapacity, ct);
            if (candidates.Count == 0)
                return new List<HallDto>();

            var busy = await _bookingRepository.GetActiveBookingsForHallsAsync(
                candidates.Select(h => h.Id), rangeStart, rangeEnd, ct);

            var busyHallIds = busy
                .Where(b => b.StartTime < rangeEnd && b.EndTime > rangeStart)
                .Select(b => b.HallId)
                .ToHashSet();

            return candidates
                .Where(h => !busyHallIds.Contains(h.Id))
                .Select(ToDto)
                .ToList();
        }

        private static void ValidateHallInput(string name, int capacity, decimal baseRate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The room name is required.");
            if (capacity <= 0)
                throw new ValidationException("The hall's capacity must be greater than zero.");
            if (baseRate <= 0)
                throw new ValidationException("The base rent must be greater than zero.");
        }

        private static void EnsureAllServicesFound(List<Guid> requestedIds, List<Amenity> found)
        {
            var missing = requestedIds.Except(found.Select(s => s.Id)).ToList();
            if (missing.Count > 0)
                throw new ValidationException($"Amenity not found: {string.Join(", ", missing)}.");
        }

        private static HallDto ToDto(Hall hall) => new(
            hall.Id,
            hall.Name,
            hall.Capacity,
            hall.BaseHourlyRate,
            hall.HallAmenities.Select(hs => new AmenityDto(hs.Amenity.Id, hs.Amenity.Name, hs.Amenity.Price)).ToList());
    }
}
