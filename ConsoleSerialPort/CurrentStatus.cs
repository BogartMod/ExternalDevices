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
        static DateTime UpdateDataTime { get; set; } 


        static CurrentStatus()
        {

        }

        static void Update()
        {
            string[] dataXY = izmDiam.GetData().Split(' ');
            Int32.TryParse(dataXY[0],out diamX);
            DiamY = dataXY[1];

        }

    }
}
