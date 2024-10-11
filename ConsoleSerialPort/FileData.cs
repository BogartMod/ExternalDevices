using ConsoleSerialPort.DTOClass;
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
            public int DiamX {  get; set; }
            public int DiamY { get; set; }
            public int DiamMean { get; set; }
            public int Eccentricity { get; set; }
            public int CurrentDistance { get; set; } = 0;
            public int CurrentSpeed { get; set; }
            public string? CurrentTime { get; set; }
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

        public void SaveData()
        {
            var package = new FileData.DataPackage
            {
                DiamX = CurrentStatus.DiamX,
                DiamY = CurrentStatus.DiamY,
                DiamMean = CurrentStatus.DiamMean,
                Eccentricity = CurrentStatus.Eccentricity,
                CurrentSpeed = CurrentStatus.Speed,
                CurrentDistance = CurrentStatus.Length,
                CurrentTime = CurrentStatus.UpdateDTIzmer.ToLongDateString(),
            };
            Data.Add(package);
        }

        public async Task SaveToFileAsync()
        {
            try
            {
                using (FileStream fileStream = new FileStream("files/" + DateTime.Now.ToString("yy-MM-dd-HH-mm") + ".json", FileMode.Append))
                {

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        IgnoreReadOnlyProperties = false,

                    };
                    await JsonSerializer.SerializeAsync<FileData>(fileStream, this);

#if DEBUG
                    Console.WriteLine("Данные сохранены в файл.");
#endif
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сохранения данных");
                Console.WriteLine(ex.ToString());
                LED.On(LedColor.Red);
            }
#if DEBUG
            Console.WriteLine("записали");
#endif
        }
    }
}
