namespace src.Models.Domain
{
    public class HallAmenity
    {
        public Guid HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public Guid AmenityId { get; set; }
        public Amenity Amenity { get; set; } = null!;
    }
}
