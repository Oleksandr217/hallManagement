using src.Models.Domain;

namespace src
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Halls.Any() || context.Amenities.Any())
                return;

            var projector = new Amenity { Id = Guid.NewGuid(), Name = "Projector", Price = 500m };
            var wifi = new Amenity { Id = Guid.NewGuid(), Name = "Wi-Fi", Price = 300m };
            var sound = new Amenity { Id = Guid.NewGuid(), Name = "Sound", Price = 700m };

            context.Amenities.AddRange(projector, wifi, sound);

            var hallA = new Hall
            {
                Id = Guid.NewGuid(),
                Name = "Hall А",
                Capacity = 50,
                BaseHourlyRate = 2000m,
                CreatedAt = DateTime.UtcNow,
                HallAmenities = new List<HallAmenity>
            {
                new() { AmenityId = projector.Id },
                new() { AmenityId = wifi.Id },
            }
            };

            var hallB = new Hall
            {
                Id = Guid.NewGuid(),
                Name = "Hall B",
                Capacity = 100,
                BaseHourlyRate = 3500m,
                CreatedAt = DateTime.UtcNow,
                HallAmenities = new List<HallAmenity>
            {
                new() { AmenityId = projector.Id },
                new() { AmenityId = wifi.Id },
                new() { AmenityId = sound.Id },
            }
            };

            var hallC = new Hall
            {
                Id = Guid.NewGuid(),
                Name = "Hall C",
                Capacity = 30,
                BaseHourlyRate = 1500m,
                CreatedAt = DateTime.UtcNow,
                HallAmenities = new List<HallAmenity>
            {
                new() { AmenityId = wifi.Id },
            }
            };

            context.Halls.AddRange(hallA, hallB, hallC);
            context.SaveChanges();
        }
    }
}
