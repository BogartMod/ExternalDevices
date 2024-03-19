using Iot.Device.Card.Ultralight;
using Iot.Device.Mcp23xxx;
using Iot.Device.Mfrc522;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ConsoleSerialPort
{
    abstract class ExternalDevice
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? SerialPort { get; set; }
        public int SerialPortSpeed { get; set; }
        public bool IsConnected { get; set; }
        public bool Enabled { get; set; }
        public List<float>? Data { get; set; }
        public List<DateTime>? Date { get; set; }
        public List<string>? DataDescription { get; set; }

        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract Task StartAsync(CancellationToken token);
        public abstract void Stop();
        


    }


    class IzmDiam : ExternalDevice
    {
        private int _serial_swich; // пин управления переключением 485
        
        //private GpioOrange? _gpioOrange;
        private SerialPort? _serial485ToTTL;

        public IzmDiam(string serialPort, int serialSpeed = 9600)
        {
            Name = ConfigurationManager.AppSettings.Get("IzmDiamName");
            Description = ConfigurationManager.AppSettings.Get("IzmDiamDescription");
            _serial_swich = Int32.Parse(ConfigurationManager.AppSettings.Get("serial_swich_port"));
            SerialPort = serialPort;
            SerialPortSpeed = serialSpeed;
            Data = new List<float>();
            DataDescription = new List<string>();
            IsConnected = false;
            Enabled = false;
        }

        public override bool Connect()
        {
            //_gpioOrange = new GpioOrange(_serial_swich);
            _serial485ToTTL = SerialConnect(SerialPort, SerialPortSpeed);
            IsConnected = true;

            return true; 
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                //_gpioOrange?.ClosePin(_serial_swich);
                //_gpioOrange?.Disconnect();
                _serial485ToTTL?.Close();

                //_gpioOrange = null;
                _serial485ToTTL = null;
                IsConnected = false;
            }
            
        }

        public async override Task StartAsync(CancellationToken token)
        {
            Enabled = true;
            
            try
            {
                //_gpioOrange.Write(_serial_swich, PinValue.High);
                _serial485ToTTL.Write(SentMessage.CreateMessage(), 0, 8);
                //_gpioOrange.Write(_serial_swich, PinValue.Low);
                ReadReponse();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            
        }

        public override void Stop()
        {
            Enabled &= false;
            
        }

        public string ReadReponse()
        {
            try
            {
                var byteBuffer = this.GetData();
                //CheckResponse(byteBuffer);
                int i = byteBuffer[4] | byteBuffer[3] << 8;
                return i.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return "";
            }
            
        }

        public byte[] GetData()
        {
            int offset = 0;
            var byteBuffer = new byte[13];
            try
            {
                while (offset < byteBuffer.Length)
                {
                    offset += _serial485ToTTL.Read(byteBuffer, offset, byteBuffer.Length - offset);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            foreach (byte byt1 in byteBuffer)
            {
                Console.Write(byt1 + " ");
            }
            return byteBuffer;
        }

        static void CheckResponse(byte[] respones)
        {
            //throw new NotImplementedException();
            // здесь проверяем контрольную сумму, номер железки и номер функции
        }

        private void SendData(List<string> listDiamX, List<string> listDiamY)
        {
            foreach (var item in listDiamX) Console.WriteLine(item);
            foreach (var item in listDiamY) Console.WriteLine(item);
            
        }

        static SerialPort SerialConnect(string port, int speed)
        {
            var serial = new SerialPort(port, speed);
            serial.Handshake = Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.Open();

            return serial;

        }

        public static class SentMessage
        {

            public static byte[] CreateMessage()
            {
                byte[] message = { 0x01, 0x03, 0x00, 0x63, 0x00, 0x04 };
                byte[] crc = BitConverter.GetBytes(ModbusCRC(message));
                byte[] messageFull = {message[0], 
                                    message[1], 
                                    message[2], 
                                    message[3], 
                                    message[4], 
                                    message[5], 
                                    crc[0], crc[1] };

                return messageFull;
            }

            public static ushort ModbusCRC(byte[] buf)
            {
                ushort crc = 0xFFFF;
                int len = buf.Length;

                for (int pos = 0; pos < len; pos++)
                {
                    crc ^= buf[pos];

                    for (int i = 8; i != 0; i--)
                    {
                        if ((crc & 0x0001) != 0)
                        {
                            crc >>= 1;
                            crc ^= 0xA001;
                        }
                        else
                            crc >>= 1;
                    }
                }

                // lo-hi
                return crc;

                // ..or
                // hi-lo reordered
                //return (ushort)((crc >> 8) | (crc << 8));
            }

            private static Int16 CreateRegValue(byte hiByte, byte lowByte)
            {
                return (Int16)(hiByte << 8 | lowByte);
            }

        }

    }
}
