using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    internal class ButtonStartStop
    {
        private GpioController? _gpioOrange;
        int _pin;

        private PinValue previousValue;

        private DateTime PreviousPushTime { get; set; }

        public ButtonStartStop()
        {

            _pin = Int32.Parse(ConfigurationManager.AppSettings.Get("ButtonStartStopPort"));
            _gpioOrange = new GpioController();
            _gpioOrange.OpenPin(_pin);
            _gpioOrange.SetPinMode(_pin, PinMode.Input);
            PreviousPushTime = DateTime.Now;
        }


        public string GetData()
        {
            bool currentButtonStatus = _gpioOrange!.Read(_pin) == PinValue.Low ? true : false;

            if (currentButtonStatus && (PreviousPushTime < DateTime.Now.AddMilliseconds(-300)))
            {
                PreviousPushTime = DateTime.Now;
                return "true";
            }
            return "false";
        }
    }
}
