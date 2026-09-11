namespace src.Models.Domain
{
    public class Amenity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<HallAmenity> HallAmenities { get; set; } = new List<HallAmenity>();
        public ICollection<BookingAmenity> BookingAmenities { get; set; } = new List<BookingAmenity>();
    }
}
