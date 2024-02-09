using Iot.Device.Mcp23xxx;
using Iot.Device.Mfrc522;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialPort
{
    abstract class ExternalDevice
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialPort { get; init; }
        public int SerialPortSpeed { get; init; }
        public bool IsConnected { get; set; }
        public bool Enabled { get; set; }
        public List<float> Data { get; set; }
        public List<string> DataDescription { get; set; }

        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract void Start();
        public abstract void Stop();
        public abstract string GetData();


    }


    class IzmDiam : ExternalDevice
    {
        private int _serial_swich = 75;
        private GpioOrange? _gpioOrange;
        private SerialPort? _serial485ToTTL;

        public IzmDiam(string serialPort, int serialSpeed)
        {
            SerialPort = serialPort;
            SerialPortSpeed = serialSpeed;
            Data = new List<float>();
            DataDescription = new List<string>();
            IsConnected = false;
            Enabled = false;
        }

        public override bool Connect()
        {
            _gpioOrange = new GpioOrange(_serial_swich);
            _serial485ToTTL = SerialConnect(SerialPort, SerialPortSpeed);
            IsConnected = true;

            return true; 
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            if (IsConnected) _gpioOrange?.Disconnect();
            _gpioOrange = null;
            _serial485ToTTL = null;
            throw new NotSupportedException(); 
        }

        public override void Start()
        {
            Enabled |= true;
            int dataVolume = 100;
            var listData = new List<string>(dataVolume);
            var listDiamX = new List<string>(dataVolume);
            var listDiamY = new List<string>(dataVolume);
            while (Enabled)
            {
                for (int i = 0; i < listData.Capacity; i++) 
                    listData[i] = GetData();

                SendData(listDiamX, listDiamY);
            }
            
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            Enabled &= false;
            throw new NotImplementedException();
        }

        
        public override string GetData()
        {
            var messageToSend = SentMessage.CreateMessage();

            return "1";
        }

        private void SendData(List<string> listDiamX, List<string> listDiamY)
        {
            foreach (var item in listDiamX) Console.WriteLine(item);
            foreach (var item in listDiamY) Console.WriteLine(item);
            throw new NotImplementedException();
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
                byte[] messageFull = { message[0], message[1], message[2], message[3], message[4], message[5], crc[0], crc[1] };

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
