using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    class FileData
    {
        public static string LineName;
        public static int DataCapacity { get; set; }
        public static List<string> ListData { get; set; }
        public static List<string> ListDiamX { get; set; }
        public static List<string> ListDiamY { get; set; }
        public static List<DateTime> ListDateTimeData { get; set; }
        public static List<string> ListLength {  get; set; }
        public static List<string> ListSpeed { get; set; }

        public FileData()
        {
            LineName = ConfigurationManager.AppSettings.Get("LineName");
            DataCapacity = Int32.Parse(ConfigurationManager.AppSettings.Get("IzmDiamCapacityStack"));
            ListData = new List<string>(DataCapacity);
            ListDiamX = new List<string>(DataCapacity);
            ListDiamY = new List<string>(DataCapacity);
            ListDateTimeData = new List<DateTime>(DataCapacity);
            ListLength = new List<string>(DataCapacity);
            ListSpeed = new List<string>(DataCapacity);
        }

    }
}
