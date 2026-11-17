using System;
using System.Collections.Generic;
using System.Text;

namespace AirWatch.Aqi.Contracts
{
    public class Reading
    {
        public Pollutant Pollutant { get; set; }

        public double Concentration { get; set; }
    }
}
