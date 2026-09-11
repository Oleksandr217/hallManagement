using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using src.Models.Domain;

namespace src.Configurations
{
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
            builder.Property(h => h.BaseHourlyRate).HasColumnType("numeric(18,2)");

            builder.HasQueryFilter(h => !h.IsDeleted);

            builder.HasMany(h => h.Bookings)
                .WithOne(b => b.Hall)
                .HasForeignKey(b => b.HallId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Price).HasColumnType("numeric(18,2)");
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }

    public class HallAmenityConfiguration : IEntityTypeConfiguration<HallAmenity>
    {
        public void Configure(EntityTypeBuilder<HallAmenity> builder)
        {
            builder.HasKey(hs => new { hs.HallId, hs.AmenityId });

            builder.HasOne(hs => hs.Hall)
                .WithMany(h => h.HallAmenities)
                .HasForeignKey(hs => hs.HallId);

            builder.HasOne(hs => hs.Amenity)
                .WithMany(s => s.HallAmenities)
                .HasForeignKey(hs => hs.AmenityId);
        }
    }

    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.TotalCost).HasColumnType("numeric(18,2)");
            builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

            builder.Property(b => b.StartTime).HasColumnType("timestamp without time zone");
            builder.Property(b => b.EndTime).HasColumnType("timestamp without time zone");

            builder.Property(b => b.CreatedAt)
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.HasIndex(b => new { b.HallId, b.StartTime, b.EndTime });
        }
    }

    public class BookingAmenityConfiguration : IEntityTypeConfiguration<BookingAmenity>
    {
        public void Configure(EntityTypeBuilder<BookingAmenity> builder)
        {
            builder.HasKey(bs => new { bs.BookingId, bs.AmenityId });
            builder.Property(bs => bs.PriceAtBooking).HasColumnType("numeric(18,2)");

            builder.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingAmenities)
                .HasForeignKey(bs => bs.BookingId);

            builder.HasOne(bs => bs.Amenity)
                .WithMany(s => s.BookingAmenities)
                .HasForeignKey(bs => bs.AmenityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
