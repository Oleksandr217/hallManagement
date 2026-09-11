using src.Interfaces;
using src.Models.Domain;
using src.Models.DTO;

namespace src.Services
{
    public class PricingService : IPricingService
    {
        private static readonly (TimeSpan From, TimeSpan To, string Name, decimal Multiplier)[] DayTariffs =
        {
        (TimeSpan.FromHours(6),  TimeSpan.FromHours(9),  "Morning (10% discount)", 0.90m),
        (TimeSpan.FromHours(9),  TimeSpan.FromHours(12), "Standard",               1.00m),
        (TimeSpan.FromHours(12), TimeSpan.FromHours(14), "Peak (15% markup)",      1.15m),
        (TimeSpan.FromHours(14), TimeSpan.FromHours(18), "Standard",               1.00m),
        (TimeSpan.FromHours(18), TimeSpan.FromHours(23), "Evening (20% discount)", 0.80m),
    };

        private static readonly TimeSpan EarliestAllowed = TimeSpan.FromHours(6);
        private static readonly TimeSpan LatestAllowed = TimeSpan.FromHours(23);

        public PricingResult CalculateHallRent(Hall hall, DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ValidationException("The reservation end time must be later than the start time.");

            if (start.Date != end.Date)
                throw new ValidationException("A reservation cannot span two calendar days. Please split it into several reservations.");

            if (start.TimeOfDay < EarliestAllowed || end.TimeOfDay > LatestAllowed)
                throw new ValidationException(
                    $"Reservations are only possible within {EarliestAllowed:hh\\:mm}–{LatestAllowed:hh\\:mm}.");

            var segments = new List<PricingSegmentDto>();
            decimal totalCost = 0m;

            foreach (var tariff in DayTariffs)
            {
                var tariffStart = start.Date + tariff.From;
                var tariffEnd = start.Date + tariff.To;

                var segmentStart = start > tariffStart ? start : tariffStart;
                var segmentEnd = end < tariffEnd ? end : tariffEnd;

                if (segmentEnd <= segmentStart)
                    continue;

                var hours = (decimal)(segmentEnd - segmentStart).TotalHours;
                var rate = hall.BaseHourlyRate * tariff.Multiplier;
                var cost = Math.Round(hours * rate, 2);

                segments.Add(new PricingSegmentDto(segmentStart, segmentEnd, tariff.Name, rate, cost));
                totalCost += cost;
            }

            return new PricingResult(segments, totalCost);
        }
    }
}
