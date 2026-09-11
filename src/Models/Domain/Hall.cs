namespace src.Models.Domain
{
    public class Hall
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal BaseHourlyRate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<HallAmenity> HallAmenities { get; set; } = new List<HallAmenity>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
