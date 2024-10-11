using ConsoleSerialPort.DTOClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    internal class Encoder
    {
        private GpioController? _gpioOrange;
        int countMeashurements;
        int _pinA;
        int _encoderResolution;
        int _encoderLengthRoll;
        int Length {  get; set; }
        int Speed { get; set; }

        public Encoder()
        {
            Int32.TryParse(
                ConfigurationManager.AppSettings.Get("EncoderNumberOfMeasurements"),
                out countMeashurements);
            Int32.TryParse(
                ConfigurationManager.AppSettings.Get("EncoderPortA"),
                out _pinA);

            _gpioOrange = new GpioController();
            _gpioOrange.OpenPin(_pinA, PinMode.Input);

            Int32.TryParse(
                ConfigurationManager.AppSettings.Get("EncoderResolution"), 
                out _encoderResolution);
            Int32.TryParse(
                ConfigurationManager.AppSettings.Get("EncoderLengthRoll"), 
                out _encoderLengthRoll);
        }

        public string GetData()
        {
            List<DateTime> meashurements = new List<DateTime>();
            DateTime dateTimeStart = DateTime.Now;
            PinValue PinValuePrevious = false;
            PinValue pinValueCurrent = false;
            int count = 0;
            int length;
            int speed;
            while (DateTime.Now < dateTimeStart.AddSeconds(1))
            {
                pinValueCurrent = _gpioOrange!.Read(_pinA);
                if (pinValueCurrent != PinValuePrevious)
                {
                    PinValuePrevious = pinValueCurrent;
                    count++;
                }
            }
            length = _encoderLengthRoll * count / _encoderResolution;
            speed = length * 60;

            return length.ToString() + " " + speed.ToString();
        }

        public EncoderDTO GetDTO()
        {
            var dto = new EncoderDTO();
            List<DateTime> meashurements = new List<DateTime>();
            DateTime dateTimeStart = DateTime.Now;
            PinValue PinValuePrevious = false;
            PinValue pinValueCurrent = false;
            int count = 0;

            while (DateTime.Now < dateTimeStart.AddSeconds(1))
            {
                pinValueCurrent = _gpioOrange!.Read(_pinA);
                if (pinValueCurrent != PinValuePrevious)
                {
                    PinValuePrevious = pinValueCurrent;
                    count++;
                }
            }

            dto.Length = _encoderLengthRoll * count / _encoderResolution;
            dto.Speed = dto.Length * 60;

            return dto;        }

    }
}
