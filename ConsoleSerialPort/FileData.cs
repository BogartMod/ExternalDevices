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
            public int CurrentDistance { get; set; } = 0;
            public string CurrentSpeed { get; set; }
            public string CurrentTime { get; set; }
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
            try
            {
                using (FileStream fs = new FileStream( DateTime.Now.ToString("yy-MM-dd-hh") + ".json", FileMode.Append))
                {

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        IgnoreReadOnlyProperties = false,

                    };
                    await JsonSerializer.SerializeAsync<FileData>(fs, this);
                    Console.WriteLine("Данные сохранены в файл.");
                }
                
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString());
                LED.On(LedColor.Red);
            }
#if DEBUG
            Console.WriteLine("записали");
#endif
        }
    }
}
