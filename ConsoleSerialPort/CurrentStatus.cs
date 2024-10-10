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
        static int Speed { get; set; } = 0;
        static int Length { get; set; } = 0;
        static int DiamX { get; set; } = 0;
        static int DiamY { get; set; } = 0;
        static int DiamMean { get; set; } = 0;
        static int Eccentricity { get; set; } = 0;
        static DateTime UpdateDTIzmer { get; set; }
        static DateTime UpdateDTEncod { get; set; }


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
