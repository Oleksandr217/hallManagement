using Microsoft.EntityFrameworkCore;
using src.Interfaces;
using src.Models.Domain;

namespace src.Repositories
{
    public class HallRepository : IHallRepository
    {
        private readonly AppDbContext _context;

        public HallRepository(AppDbContext context) => _context = context;

        public Task<Hall?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _context.Halls
                .Include(h => h.HallAmenities).ThenInclude(hs => hs.Amenity)
                .FirstOrDefaultAsync(h => h.Id == id, ct);

        public Task<List<Hall>> GetAllAsync(CancellationToken ct = default) =>
            _context.Halls
                .Include(h => h.HallAmenities).ThenInclude(hs => hs.Amenity)
                .OrderBy(h => h.Name)
                .ToListAsync(ct);

        public Task<List<Hall>> GetByMinCapacityAsync(int minCapacity, CancellationToken ct = default) =>
            _context.Halls
                .Include(h => h.HallAmenities).ThenInclude(hs => hs.Amenity)
                .Where(h => h.Capacity >= minCapacity)
                .OrderBy(h => h.Capacity)
                .ToListAsync(ct);

        public async Task AddAsync(Hall hall, CancellationToken ct = default) =>
            await _context.Halls.AddAsync(hall, ct);

        public void Update(Hall hall) => _context.Halls.Update(hall);

        public void Delete(Hall hall)
        {
            hall.IsDeleted = true;
            _context.Halls.Update(hall);
        }

        public Task<List<Amenity>> GetAmenitiesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default) =>
            _context.Amenities.Where(s => ids.Contains(s.Id)).ToListAsync(ct);

        public Task SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
        public Task<Hall?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct = default) =>
            _context.Halls
            .IgnoreQueryFilters()
            .Include(h => h.HallAmenities).ThenInclude(ha => ha.Amenity)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }
}
