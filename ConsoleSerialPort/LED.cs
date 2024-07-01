using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    internal static class LED
    {
        private static GpioController? _gpioOrange;
        static List<int> _ledsPin;
        static bool isBlink = false;

        static LED()
        {
            _ledsPin = new List<int>(3);
            _ledsPin.Add(Int32.Parse(ConfigurationManager.AppSettings.Get("LedGreen")));
            _ledsPin.Add(Int32.Parse(ConfigurationManager.AppSettings.Get("LedYellow")));
            _ledsPin.Add(Int32.Parse(ConfigurationManager.AppSettings.Get("LedRed")));

            
            _gpioOrange = new GpioController();
            foreach (int pin in _ledsPin)
                _gpioOrange.OpenPin(pin, PinMode.Output);

        }
        public static void AllOn()
        {
            AllOff();
            foreach (int pin in _ledsPin)
            {
                _gpioOrange!.Write(pin, PinValue.High);
            }
        }
        public static void AllOff ()
        {
            foreach (int pin in _ledsPin)
            {
                _gpioOrange!.Write(pin, PinValue.Low);
            }
            isBlink = false;
        }
        public static void On(LedColor color)
        {
            AllOff();
            _gpioOrange!.Write(_ledsPin[(int)color], PinValue.High);
        }
        public static void Off(LedColor color)
        {

        }

        public static async Task BlinkAsync(LedColor color)
        {
            isBlink = true;

            while (isBlink)
            {
                _gpioOrange!.Write(_ledsPin[(int)color], PinValue.High);
                await Task.Delay(1000);
                foreach (int pin in _ledsPin)
                {
                    _gpioOrange.Write(pin, PinValue.Low);
                }
                await Task.Delay(1000);
            }
        }
    }

    enum LedColor
    {
        Red = 2,
        Green = 0,
        Yellow = 1,
    }
}
