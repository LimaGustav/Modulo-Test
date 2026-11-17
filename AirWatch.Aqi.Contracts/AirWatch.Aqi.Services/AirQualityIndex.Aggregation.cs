using AirWatch.Aqi.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirWatch.Aqi.Services
{
    public sealed partial class AirQualityIndex
    {
        public int OverallAqi(IReadOnlyList<Reading> readings)
        {
            return Aggregate(readings).Aqi;
        }

        public Pollutant? DominantPollutant(IReadOnlyList<Reading> readings)
        {
            return Aggregate(readings).Dominant;
        }

        private (int Aqi, Pollutant? Dominant) Aggregate(IReadOnlyList<Reading> readings)
        {
            int highestAqi = -1;
            Pollutant? dominant = null;

            if (readings is null)
                return (highestAqi, dominant);

            foreach (Reading reading in readings)
            {
                if (reading is null)
                    continue;

                int subIndex = SubIndex(reading.Pollutant, reading.Concentration);
                if (subIndex < 0)
                    continue;

                if (subIndex > highestAqi ||
                    (subIndex == highestAqi && dominant.HasValue &&
                     (int)reading.Pollutant < (int)dominant.Value))
                {
                    highestAqi = subIndex;
                    dominant = reading.Pollutant;
                }
            }

            return (highestAqi, dominant);
        }
    }
}
