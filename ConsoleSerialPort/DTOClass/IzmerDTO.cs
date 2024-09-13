using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort.DTOClass
{
    internal class IzmerDTO
    {
        int DiamX { get; set; } = 0;
        int DiamY { get; set; } = 0;
        int DiamMean { get; set; } = 0;
        int Eccentricity { get; set; } = 0;
        DateTime UpdateDataTime { get; set; }
    }
}
