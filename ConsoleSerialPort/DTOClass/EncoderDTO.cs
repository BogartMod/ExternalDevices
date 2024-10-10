using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort.DTOClass
{
    internal class EncoderDTO
    {
        public int Length { get; set; }
        public int Speed { get; set; }
        public DateTime UpdateDataTime { get; set; }
    }
}
