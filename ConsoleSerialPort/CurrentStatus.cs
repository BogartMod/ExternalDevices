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
        static int diamX { get; set; } = 0;
        static int diamY { get; set; } = 0;
        static int diamMean { get; set; } = 0;
        static int eccentricity { get; set; } = 0;


        static CurrentStatus()
        {

            Speed = 0;
            Length = 0;
            diamMean = 0;
            eccentricity = 0;
            diamX = 0;
            diamY = 0;

        }

        static void Update()
        {

        }

    }
}
