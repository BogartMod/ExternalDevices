using System;
using System.IO.Ports;
using Iot.Device;
using System.Device.Gpio;



namespace ConsoleSerialPort
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var izmDiam = new IzmDiam("/dev/ttyS5", 9600);

            //ExternalDevice externalDevice = new ExternalDevice();
            //externalDevice.Connect();
            //externalDevice.Start();
            //externalDevice.Stop();



            int pc11 = 75; //485 управляющий пин
            var gpioOrange = new GpioOrange(pc11);
            //var gpioOrange = new GpioOrange();

            //var serial = SerialConnect("COM13", 9600);


            //var serialRS485toTTL = SerialConnect("/dev/ttyS5", 9600);
            //var serial485 = new Serial485("/dev/ttys5");

            

            //var messageToSend = CreateMessage();

            gpioOrange.Write(pc11, PinValue.High);
            
            serialRS485toTTL.Write(messageToSend, 0, 8);

            gpioOrange.Write(pc11, PinValue.Low);

            Console.WriteLine("Чтение");
            IzmDiam answ = ReadReponse(serialRS485toTTL);
            //Console.WriteLine(answ.SizeX);
            //Console.WriteLine(answ.SizeY);
            serialRS485toTTL.Close();
            

            gpioOrange.ClosePin(pc11);
        }

        
        //static SerialPort SerialConnect(string port, int speed)
        //{
        //    var serial = new SerialPort(port, speed);
        //    serial.Handshake = Handshake.None;
        //    serial.Parity = Parity.None;
        //    serial.DataBits = 8;
        //    serial.StopBits = StopBits.One;
        //    serial.Open();

        //    return serial;

        //}

        //static byte[] CreateMessage()
        //{
        //    byte[] message = { 0x01, 0x03, 0x00, 0x63, 0x00, 0x04 };
        //    byte[] crc = BitConverter.GetBytes(ModbusCRC(message));
        //    byte[] messageFull = { message[0], message[1], message[2], message[3], message[4], message[5], crc[0], crc[1] };

        //    return messageFull;
        //}

        //static IzmDiam ReadReponse(SerialPort serial)
        //{
        //    var byteBuffer = GetData(serial);
        //    CheckResponse(byteBuffer);

        //    return new IzmDiam(byteBuffer);
        //}

        static byte[] GetData(SerialPort serial)
        {
            int offset = 0;
            var byteBuffer = new byte[9];
            while (offset < 9)
            {
                offset += serial.Read(byteBuffer, offset, byteBuffer.Length - offset);
            }

            return byteBuffer;
        }

        static void CheckResponse(byte[] respones)
        {
            // здесь проверяем контрольную сумму, номер железки и номер функции
        }

        static Int16 CreateRegValue(byte hiByte, byte lowByte)
        {
            return (Int16)(hiByte << 8 | lowByte);
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

    //public class Serial485 : SerialPort
    //{
    //    string Port {  get; }

    //    Serial485()
    //    {

    //    }

    //    public Serial485(string port)
    //    {

    //    }

    //    public bool SendingMessage(byte[] message)
    //    {
    //        this.Write(message, 0, 8);
    //        return true;

    //    }

        
    //}

    //public static class SentMessage
    //{
    //    public static ushort ModbusCRC(byte[] buf)
    //    {
    //        ushort crc = 0xFFFF;
    //        int len = buf.Length;

    //        for (int pos = 0; pos < len; pos++)
    //        {
    //            crc ^= buf[pos];

    //            for (int i = 8; i != 0; i--)
    //            {
    //                if ((crc & 0x0001) != 0)
    //                {
    //                    crc >>= 1;
    //                    crc ^= 0xA001;
    //                }
    //                else
    //                    crc >>= 1;
    //            }
    //        }

    //        // lo-hi
    //        return crc;

    //        // ..or
    //        // hi-lo reordered
    //        //return (ushort)((crc >> 8) | (crc << 8));
    //    }

    //    private static Int16 CreateRegValue(byte hiByte, byte lowByte)
    //    {
    //        return (Int16)(hiByte << 8 | lowByte);
    //    }

    //}
}
