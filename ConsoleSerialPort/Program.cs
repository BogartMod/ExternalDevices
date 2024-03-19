using System;
using System.IO.Ports;
using Iot.Device;
using System.Device.Gpio;
using System.Configuration;
using System.Collections.Specialized;
using static ConsoleSerialPort.IzmDiam;
using ConsoleSerialPort;



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
                int _delayMS = Int32.Parse(ConfigurationManager.AppSettings.Get("IzmDiamDelayMS"));
                
                IzmWorkTask = izmDiam.StartAsync(token);

                try
                {
                    var fileData = new FileData();
                    while (token.IsCancellationRequested)
                    {
                        for (int i = 0; i < fileData.DataCapacity; i++)
                        {
                            fileData.ListDiamX.Add("");
                            fileData.ListDiamY.Add("");
                            await Task.Delay(_delayMS);
                        }

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
