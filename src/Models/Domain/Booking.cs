using src.Models.Enum;

namespace src.Models.Domain
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid HallId { get; set; }
        public Hall Hall { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
        public DateTime CreatedAt { get; set; }

        public ICollection<BookingAmenity> BookingAmenities { get; set; } = new List<BookingAmenity>();
    }
}
