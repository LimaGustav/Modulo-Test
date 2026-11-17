using AirWatch.Aqi.Contracts;
using System;
using System.Collections.Generic;

namespace AirWatch.Aqi.Services
{
    public sealed partial class AirQualityIndex
    {
        public int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours)
        {
            if (recentHours is null || recentHours.Count == 0)
                return -1;

            var present = new List<(int Hour, double Concentration)>();
            int recentCount = 0;
            double sum = 0;

            for (int hour = 0; hour < Math.Min(recentHours.Count, 12); hour++)
            {
                double? reading = recentHours[hour];
                if (!reading.HasValue)
                    continue;

                double concentration = reading.Value;
                if (!double.IsFinite(concentration) || concentration < 0)
                    return -1;

                present.Add((hour, concentration));
                sum += concentration;
                if (hour < 3)
                    recentCount++;
            }

            if (recentCount < 2)
                return -1;

            double mean = sum / present.Count;
            double varianceSum = 0;
            foreach (var reading in present)
                varianceSum += Math.Pow(reading.Concentration - mean, 2);

            double standardDeviation = Math.Sqrt(varianceSum / present.Count);
            double weight = mean + standardDeviation == 0
                ? 1
                : Math.Clamp(mean / (mean + standardDeviation), 0.5, 1);

            double weightedSum = 0;
            double totalWeight = 0;
            foreach (var reading in present)
            {
                double hourlyWeight = Math.Pow(weight, reading.Hour);
                weightedSum += hourlyWeight * reading.Concentration;
                totalWeight += hourlyWeight;
            }

            return SubIndex(pollutant, weightedSum / totalWeight);
        }

    }
}
