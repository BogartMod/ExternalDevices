using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    class FileData
    {
        public class DataPackage
        {
            public string DiamX {  get; set; }
            public string DiamY { get; set; }
            public string CurrentDistance { get; set; }
            public string CurrentSpeed { get; set; }
            public DateTime CurrentTime { get; set; }
        } 

        public string LineName { get; set; }
        public int DataCapacity { get; set; }
        public  List<DataPackage> Data { get; set; }

        public FileData()
        {
            LineName = ConfigurationManager.AppSettings.Get("LineName");
            DataCapacity = Int32.Parse(ConfigurationManager.AppSettings.Get("DataCapacityStack"));
            Data = new List<DataPackage>(DataCapacity);
        }

        public async Task SaveToFileAsync()
        {
            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        IgnoreReadOnlyProperties = false,

                    };
                    await JsonSerializer.SerializeAsync<FileData>(fs, this);
                    Console.WriteLine("Data has been saved to file");
                }
                catch (Exception ex )
                { Console.WriteLine(ex.ToString()); }
                
            }
        }
    }
}
