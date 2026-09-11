using Microsoft.EntityFrameworkCore;
using src.Models.Domain;

namespace src
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<HallAmenity> HallAmenities => Set<HallAmenity>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingAmenity> BookingAmenities => Set<BookingAmenity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
