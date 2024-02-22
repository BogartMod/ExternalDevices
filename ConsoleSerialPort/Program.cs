using System;
using System.IO.Ports;
using Iot.Device;
using System.Device.Gpio;



namespace ConsoleSerialPort
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var izmDiam = new IzmDiam("/dev/ttyS5", 9600);
            izmDiam.Connect();
            izmDiam.StartAsync();
            izmDiam.Stop();
            izmDiam.Disconnect();


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
