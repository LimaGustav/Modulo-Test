using AirWatch.Aqi.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirWatch.Aqi.Services
{
    public sealed partial class AirQualityIndex : IAirQualityIndex
    {
        public int SubIndex(Pollutant pollutant, double concentration)
        {
            throw new NotImplementedException();
        }
    }
}
