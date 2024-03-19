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
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            var appSettings = ConfigurationManager.AppSettings;

            Task IzmWorkTask = null;

            //var izmDiam = new IzmDiam(
            //    serialPort: ConfigurationManager.AppSettings.Get("IzmDiamPort"),
            //    serialSpeed: Int32.Parse(ConfigurationManager.AppSettings.Get("IzmDiamSpeed")) );
            var izmDiam = new IzmDiam(
                serialPort: "COM13",
                serialSpeed: Int32.Parse(
                    ConfigurationManager.AppSettings.Get("IzmDiamSpeed"))
                );

            while (true)
            {


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
                WorkAsync();
                
            }
            async Task StopAsync()
            {
                cancelTokenSource.Cancel();
                Console.WriteLine("Остановка");
                izmDiam.Stop();
                izmDiam.Disconnect();
                if (IzmWorkTask != null) await IzmWorkTask;
                Console.WriteLine("Процесс остановлен");

            }
            async Task WorkAsync()
            {
                int _delayMS = Int32.Parse(ConfigurationManager.AppSettings.Get("ReadDelayMS"));
                
                try
                {
                    var fileData = new FileData();
                    while (!token.IsCancellationRequested)
                    {
                        for (int i = 0; i < fileData.DataCapacity; i++)
                        {
                            string[] dataXY = izmDiam.GetData().Split(' ');
                            //fileData.ListDiamX.Add(dataXY[0]);
                            //fileData.ListDiamY.Add(dataXY[1]);
                            //fileData.ListDateTimeData.Add(DateTime.Now);

                            fileData.Data.Add(new FileData.DataPackage());

                            fileData.Data[i].DiamX = dataXY[0];
                            fileData.Data[i].DiamY = dataXY[1];
                            fileData.Data[i].CurrentTime = DateTime.Now;

                            Console.WriteLine(dataXY[0] + " " + dataXY[1]);
                            await Task.Delay(_delayMS);

                        }
                        cancelTokenSource.Cancel();
                        fileData.SaveToFileAsync();
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            void Exit()
            {
                cancelTokenSource.Dispose();
                Environment.Exit(0);
            }

        }

        public async Task SaveDataAsync(IzmDiam izmDiam)
        {

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
