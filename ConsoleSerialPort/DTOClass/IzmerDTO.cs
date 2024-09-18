using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort.DTOClass
{
    internal class IzmerDTO
    {
        public int DiamX { get; set; } = 0;
        public int DiamY { get; set; } = 0;
        public int DiamMean => DiamX + DiamY / 2;
        public int Eccentricity => DiamX - DiamY;
        public DateTime UpdateDataTime { get; set; }
    }
}
