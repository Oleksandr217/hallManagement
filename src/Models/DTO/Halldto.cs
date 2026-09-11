namespace src.Models.DTO
{
    public record AmenityDto(Guid Id, string Name, decimal Price);
    public record HallDto(
        Guid Id,
        string Name,
        int Capacity,
        decimal BaseHourlyRate,
        IReadOnlyList<AmenityDto> Amenitys);

    public class CreateHallDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal BaseHourlyRate { get; set; }
        public List<Guid> AmenityId { get; set; } = new();
    }

    public class UpdateHallDto
    {
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public decimal? BaseHourlyRate { get; set; }

        public List<Guid>? AmenityIds { get; set; }
    }

    public class CreateAmenityDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class HallAvailabilitySearchDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MinCapacity { get; set; }
    }
}
