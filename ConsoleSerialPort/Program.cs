using System;
using System.IO.Ports;
using Iot.Device;
using System.Device.Gpio;
using System.Configuration;
using System.Collections.Specialized;



namespace ConsoleSerialPort
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                ConsoleMenu();
            }

        }

        static async void ConsoleMenu()
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

            string cons = Console.ReadLine();

            switch (cons)
            {
                case "start":
                    Console.WriteLine("Подключаем.");
                    await Task.Run(() => izmDiam.Connect());
                    Console.WriteLine("Подключились. Начинаем записывать");
                    IzmWorkTask = izmDiam.StartAsync(token);
                    break;

                case "stop":
                    cancelTokenSource.Cancel();
                    Console.WriteLine("Остановка");
                    izmDiam.Stop();
                    izmDiam.Disconnect();
                    if (IzmWorkTask != null) await IzmWorkTask;

                    break;

                case "exit":
                    cancelTokenSource.Dispose();
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
