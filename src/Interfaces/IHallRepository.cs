using src.Models.Domain;

namespace src.Interfaces
{
    public interface IHallRepository
    {
        Task<Hall?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Hall>> GetAllAsync(CancellationToken ct = default);

        /// <summary>Returns the halls that meet the minimum capacity.</summary>
        Task<List<Hall>> GetByMinCapacityAsync(int minCapacity, CancellationToken ct = default);

        Task AddAsync(Hall hall, CancellationToken ct = default);
        void Update(Hall hall);

        /// <summary>Soft delete.</summary>
        void Delete(Hall hall);

        Task<List<Amenity>> GetAmenitiesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<Hall?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct = default);
    }
}
