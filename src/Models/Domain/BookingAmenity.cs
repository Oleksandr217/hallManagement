namespace src.Models.Domain
{
    public class BookingAmenity
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public Guid AmenityId { get; set; }
        public Amenity Amenity { get; set; } = null!;

        public decimal PriceAtBooking { get; set; }
    }
}
