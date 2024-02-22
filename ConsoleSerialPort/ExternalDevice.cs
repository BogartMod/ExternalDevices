using Iot.Device.Mcp23xxx;
using Iot.Device.Mfrc522;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<float> Data { get; set; }
        public List<string> DataDescription { get; set; }

        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract Task StartAsync();
        public abstract void Stop();
        


    }


    class IzmDiam : ExternalDevice
    {
        private int _serial_swich = 75; // пин управления переключением 485
        private GpioOrange? _gpioOrange;
        private SerialPort? _serial485ToTTL;

        public IzmDiam(string serialPort, int serialSpeed)
        {
            Name = "IzmDiam";
            Description = "IzmDiam";
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
            if (IsConnected)
            {
                _gpioOrange?.ClosePin(_serial_swich);
                _gpioOrange?.Disconnect();
                _serial485ToTTL?.Close();

                _gpioOrange = null;
                _serial485ToTTL = null;
                IsConnected = false;
            }
            
        }

        public async override Task StartAsync()
        {
            Enabled = true;
            int dataVolume = 100;
            var listData = new List<string>(dataVolume);
            var listDiamX = new List<string>(dataVolume);
            var listDiamY = new List<string>(dataVolume);
            while (Enabled)
            {
                for (int i = 0; i < listData.Capacity; i++)
                {
                    _gpioOrange.Write(_serial_swich, PinValue.High);
                    _serial485ToTTL.Write(SentMessage.CreateMessage(), 0, 8);
                    _gpioOrange.Write(_serial_swich, PinValue.Low);
                    listData[i] = ReadReponse();
                    await Task.Delay(1000);
                } 
                SendData(listDiamX, listDiamY);
            }
            
        }

        public override void Stop()
        {
            Enabled &= false;
            throw new NotImplementedException();
        }

        public string ReadReponse()
        {
            var byteBuffer = this.GetData();
            CheckResponse(byteBuffer);

            return byteBuffer.ToString();
        }

        public byte[] GetData()
        {
            int offset = 0;
            var byteBuffer = new byte[9];
            while (offset < 9)
            {
                offset += _serial485ToTTL.Read(byteBuffer, offset, byteBuffer.Length - offset);
            }

            return byteBuffer;
        }

        static void CheckResponse(byte[] respones)
        {
            throw new NotImplementedException();
            // здесь проверяем контрольную сумму, номер железки и номер функции
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
