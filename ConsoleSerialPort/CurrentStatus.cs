using ConsoleSerialPort.DTOClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    static internal class CurrentStatus
    {
        public static int Speed { get; set; } = 0;
        public static int Length { get; set; } = 0;
        public static int DiamX { get; set; } = 0;
        public static int DiamY { get; set; } = 0;
        public static int DiamMean { get; set; } = 0;
        public static int Eccentricity { get; set; } = 0;
        public static DateTime UpdateDTIzmer { get; set; }
        public static DateTime UpdateDTEncod { get; set; }


        static CurrentStatus()
        {

        }

        public static void UpdateIzmer(IzmerDTO izmerDTO)
        {
            DiamX = izmerDTO.DiamX;
            DiamY = izmerDTO.DiamY;
            DiamMean = izmerDTO.DiamMean;
            Eccentricity = izmerDTO.Eccentricity;
        }

        public static void UpdateEncoder(EncoderDTO encoderDTO)
        {
            Speed = encoderDTO.Speed; 
            Length = encoderDTO.Length;
            UpdateDTEncod = encoderDTO.UpdateDataTime;
        }

    }
}
