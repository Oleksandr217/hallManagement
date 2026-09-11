using src.Models.Domain;
using src.Services;
using Xunit;

namespace src.Tests
{
    public class PricingServiceTests
    {
        private readonly PricingService _sut = new();

        private static Hall MakeHall(decimal baseRate = 2000m) => new()
        {
            Id = Guid.NewGuid(),
            Name = "Зал А",
            Capacity = 50,
            BaseHourlyRate = baseRate
        };

        [Fact]
        public void StandardHours_ChargesBaseRate()
        {
            var hall = MakeHall(2000m);
            var start = new DateTime(2024, 9, 1, 9, 0, 0);
            var end = new DateTime(2024, 9, 1, 11, 0, 0);

            var result = _sut.CalculateHallRent(hall, start, end);

            Assert.Equal(4000m, result.HallRentCost);
            Assert.Single(result.Breakdown);
        }

        [Fact]
        public void MorningHours_Apply10PercentDiscount()
        {
            var hall = MakeHall(2000m);
            var start = new DateTime(2024, 9, 1, 6, 0, 0);
            var end = new DateTime(2024, 9, 1, 9, 0, 0); 

            var result = _sut.CalculateHallRent(hall, start, end);

            Assert.Equal(5400m, result.HallRentCost);
        }

        [Fact]
        public void EveningHours_Apply20PercentDiscount()
        {
            var hall = MakeHall(2000m);
            var start = new DateTime(2024, 9, 1, 18, 0, 0);
            var end = new DateTime(2024, 9, 1, 20, 0, 0); 

            var result = _sut.CalculateHallRent(hall, start, end);

            Assert.Equal(3200m, result.HallRentCost);
        }

        [Fact]
        public void PeakHours_Apply15PercentSurcharge()
        {
            var hall = MakeHall(2000m);
            var start = new DateTime(2024, 9, 1, 12, 0, 0);
            var end = new DateTime(2024, 9, 1, 14, 0, 0); 

            var result = _sut.CalculateHallRent(hall, start, end);

            Assert.Equal(4600m, result.HallRentCost);
        }

        [Fact]
        public void BookingSpanningMultipleTariffZones_SplitsIntoCorrectSegments()
        {
            var hall = MakeHall(2000m);
            
            var start = new DateTime(2024, 9, 1, 8, 0, 0);
            var end = new DateTime(2024, 9, 1, 15, 0, 0);

            var result = _sut.CalculateHallRent(hall, start, end);

            Assert.Equal(14400m, result.HallRentCost);
            Assert.Equal(4, result.Breakdown.Count);
        }

        [Fact]
        public void EndBeforeStart_ThrowsValidationException()
        {
            var hall = MakeHall();
            var start = new DateTime(2024, 9, 1, 12, 0, 0);
            var end = new DateTime(2024, 9, 1, 10, 0, 0);

            Assert.Throws<ValidationException>(() => _sut.CalculateHallRent(hall, start, end));
        }

        [Fact]
        public void BookingOutsideAllowedRange_ThrowsValidationException()
        {
            var hall = MakeHall();
            var start = new DateTime(2024, 9, 1, 4, 0, 0); 
            var end = new DateTime(2024, 9, 1, 8, 0, 0);

            Assert.Throws<ValidationException>(() => _sut.CalculateHallRent(hall, start, end));
        }

        [Fact]
        public void BookingCrossingMidnight_ThrowsValidationException()
        {
            var hall = MakeHall();
            var start = new DateTime(2024, 9, 1, 22, 0, 0);
            var end = new DateTime(2024, 9, 2, 1, 0, 0);

            Assert.Throws<ValidationException>(() => _sut.CalculateHallRent(hall, start, end));
        }
    }
}
