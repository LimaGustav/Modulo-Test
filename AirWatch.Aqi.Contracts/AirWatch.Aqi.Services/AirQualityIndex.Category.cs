using System;
using System.Collections.Generic;
using System.Text;

namespace AirWatch.Aqi.Services
{
    public sealed partial class AirQualityIndex
    {
        public string Category(int aqi)
        {
            return aqi switch
            {
                < 0 => "desconhecido",
                <= 50 => "Bom",
                <= 100 => "Moderado",
                <= 150 => "USG",
                <= 200 => "Insalubre",
                <= 300 => "Muito Insalubre",
                _ => "Perigoso"
            };
        }

    }
}
