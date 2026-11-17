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
            throw new NotImplementedException();
        }
        public Pollutant? DominantPollutant(IReadOnlyList<Reading> readings)
        {
            throw new NotImplementedException();
        }
    }
}
