using src.Models.DTO;

namespace src.Interfaces
{
    public interface IHallService
    {
        Task<HallDto> CreateAsync(CreateHallDto dto, CancellationToken ct = default);
        Task<HallDto> UpdateAsync(Guid id, UpdateHallDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<HallDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<HallDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>Search for rooms that are available during a specified time period and have sufficient capacity.</summary>
        Task<List<HallDto>> SearchAvailableAsync(HallAvailabilitySearchDto searchDto, CancellationToken ct = default);
    }
}
