using AirWatch.Aqi.Contracts;
using System;

namespace AirWatch.Aqi.Services
{
    public sealed partial class AirQualityIndex : IAirQualityIndex
    {
        private readonly record struct ConcentrationRange(
            double Lower, double Upper, int LowerIndex, int UpperIndex);

        private static readonly ConcentrationRange[] Pm25Ranges =
        {
            new(0, 12, 0, 50),
            new(12, 35, 51, 100),
            new(35, 55, 101, 150),
            new(55, 150, 151, 200),
            new(150, 250, 201, 300),
            new(250, 500, 301, 500)
        };

        private static readonly ConcentrationRange[] Pm10Ranges =
        {
            new(0, 54, 0, 50),
            new(54, 154, 51, 100),
            new(154, 254, 101, 150),
            new(254, 354, 151, 200),
            new(354, 424, 201, 300),
            new(424, 604, 301, 500)
        };

        private static readonly ConcentrationRange[] O3Ranges =
        {
            new(0, 54, 0, 50),
            new(54, 70, 51, 100),
            new(70, 85, 101, 150),
            new(85, 105, 151, 200),
            new(105, 200, 201, 300)
        };

        public int SubIndex(Pollutant pollutant, double concentration)
        {
            if (!double.IsFinite(concentration) || concentration < 0 ||
                !TryGetPollutantTable(pollutant, out var ranges, out var multiplier))
                return -1;

            if (concentration >= ranges[^1].Upper + (double)(1m / multiplier))
                return -1;

            double reportedConcentration = TruncateConcentration(concentration, multiplier);
            return TryGetConcentrationRange(ranges, reportedConcentration, out var range)
                ? CalculateSubIndex(range, reportedConcentration)
                : -1;
        }

        private static bool TryGetPollutantTable(Pollutant pollutant,
            out ConcentrationRange[] ranges, out decimal multiplier)
        {
            (ranges, multiplier) = pollutant switch
            {
                Pollutant.Pm25 => (Pm25Ranges, 1m),
                Pollutant.Pm10 => (Pm10Ranges, 100m),
                Pollutant.O3 => (O3Ranges, 10m),
                _ => (Array.Empty<ConcentrationRange>(), 1m)
            };

            return ranges.Length > 0;
        }

        private static bool TryGetConcentrationRange(ConcentrationRange[] ranges,
            double concentration, out ConcentrationRange range)
        {
            foreach (var candidate in ranges)
            {
                if (concentration >= candidate.Lower && concentration <= candidate.Upper)
                {
                    range = candidate;
                    return true;
                }
            }

            range = default;
            return false;
        }

        private static double TruncateConcentration(double concentration, decimal multiplier)
        {
            decimal truncated = decimal.Truncate((decimal)concentration * multiplier)
                / multiplier;

            // The double-to-decimal conversion can round just below a boundary up.
            if ((double)truncated > concentration)
                truncated -= 1m / multiplier;

            return (double)truncated;
        }

        private static int CalculateSubIndex(ConcentrationRange range, double concentration)
        {
            double value = range.LowerIndex +
                (range.UpperIndex - range.LowerIndex) *
                (concentration - range.Lower) / (range.Upper - range.Lower);
                
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
