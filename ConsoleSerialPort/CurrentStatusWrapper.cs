using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    internal class CurrentStatusWrapper
    {
        public static int Speed { get; set; } = 0;
        public static int Length { get; set; } = 0;
        public static int DiamX { get; set; } = 0;
        public static int DiamY { get; set; } = 0;
        public static int DiamMean { get; set; } = 0;
        public static int Eccentricity { get; set; } = 0;
        public static DateTime UpdateDTIzmer { get; set; }
        public static DateTime UpdateDTEncod { get; set; }


        public CurrentStatusWrapper()
        {
            UpdateData();
        }

        public void UpdateData()
        {
            Speed = CurrentStatus.Speed;
            Length = CurrentStatus.Length;
            DiamX = CurrentStatus.DiamX;
            DiamY = CurrentStatus.DiamY;
            DiamMean = CurrentStatus.DiamMean;
            Eccentricity = CurrentStatus.Eccentricity;
            UpdateDTEncod = CurrentStatus.UpdateDTEncod;
            UpdateDTIzmer = CurrentStatus.UpdateDTIzmer;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize<CurrentStatusWrapper>(this);
        }
    }
}
