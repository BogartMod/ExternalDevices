using System;
using System.IO.Ports;
using Iot.Device;
using System.Device.Gpio;
using System.Configuration;
using System.Collections.Specialized;
using static ConsoleSerialPort.IzmDiam;
using ConsoleSerialPort;
using System.Runtime.CompilerServices;



namespace ConsoleSerialPort
{
    class Program
    {
        static void Main(string[] args)
        {
            //CancellationTokenSource cancelTokenSource;
            //CancellationToken token = cancelTokenSource.Token;


            var appSettings = ConfigurationManager.AppSettings;

            Task IzmWorkTask = null;

            var izmDiam = new IzmDiam(
                serialPort: ConfigurationManager.AppSettings.Get("IzmDiamPort"),
                serialSpeed: Int32.Parse(ConfigurationManager.AppSettings.Get("IzmDiamSpeed")) );
            //var izmDiam = new IzmDiam(
            //    serialPort: "COM13",
            //    serialSpeed: Int32.Parse(
            //        ConfigurationManager.AppSettings.Get("IzmDiamSpeed")) );

            var encoder = new Encoder();

            while (true)
            {

                Console.WriteLine("Ожидание комманды.");
                string cons = Console.ReadLine();

                switch (cons)
                {
                    case "start":
                        Start();
                        break;

                    case "stop":
                        StopAsync();
                        break;

                    case "exit":
                        Exit();
                        break;

                    case "help":
                        Console.WriteLine("start - запуск отслеживания");
                        Console.WriteLine("stop - остановка ослеживания");
                        Console.WriteLine("exit - завершить программу");
                        Console.WriteLine("help - вывод подсказки");
                        break;

                    default:
                        Console.WriteLine("Неизвестная комманда. Список комманд: help");
                        break;
                }
            }

            void Start()
            {
                Console.WriteLine("Подключаем.");
                izmDiam.Connect();
                Console.WriteLine("Подключились. Начинаем записывать");
                izmDiam.IsEnabled = true;
                WorkAsync();
                
            }
            async Task StopAsync()
            {
                Console.WriteLine("Остановка");
                izmDiam.Stop();
                izmDiam.Disconnect();
                if (IzmWorkTask != null) await IzmWorkTask;
                Console.WriteLine("Процесс остановлен");

            }
            async Task WorkAsync()
            {
                int _delayMS = Int32.Parse(ConfigurationManager.AppSettings.Get("ReadDelayMS"));
                int currentLength = 0;
                try
                {
                    var fileData = new FileData();
                    while (izmDiam.IsEnabled)
                    {
                        for (int i = 0; i < fileData.DataCapacity; i++)
                        {
                            string[] dataXY = izmDiam.GetData().Split(' ');

                            fileData.Data.Add(new FileData.DataPackage());
                            fileData.Data[i].DiamX = dataXY[0];
                            fileData.Data[i].DiamY = dataXY[1];
                            fileData.Data[i].CurrentTime = DateTime.Now.ToString("O");

                            string[] dataEncoder = encoder.GetData().Split(' ');
                            currentLength += Int32.Parse(dataEncoder[0]);
                            fileData.Data[i].CurrentDistance = currentLength;
                            fileData.Data[i].CurrentSpeed = dataEncoder[1];


#if DEBUG
                            Console.WriteLine(dataXY[0]);
                            Console.WriteLine(currentLength);
                            Console.WriteLine(dataEncoder[1]);
#endif

                            await Task.Delay(_delayMS);

                        }
                        izmDiam.Stop(); //todo:test
                        fileData.SaveToFileAsync();
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            async Task WaitPressButtonAsync()
            {

            }

            void Exit()
            {
                izmDiam.Stop();
                izmDiam.Disconnect();
                Environment.Exit(0);
            }

        }

    }



    public class GpioOrange : GpioController
    {
        public byte[]? Data { get; set; }
        int Pin { get; set; }
        GpioController GpioController { get; set; }

        public GpioOrange()
        {
            GpioController = new GpioController();
            Pin = 0;
        }

        public GpioOrange(int pin)
        {
            Pin = pin;
            GpioController = new GpioController();
            GpioController.OpenPin(Pin);
            
        }

        public bool Disconnect()
        {
            if (Pin != 0) GpioController.ClosePin(Pin);
            return true;
        }
    }

    
}
