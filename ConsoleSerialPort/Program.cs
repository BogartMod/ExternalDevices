using System;
using System.IO.Ports;
using Iot.Device;
using System.Net;
using System.Text;
using System.Device.Gpio;
using System.Configuration;
using System.Collections.Specialized;
using static ConsoleSerialPort.IzmDiam;
using ConsoleSerialPort;
using System.Runtime.CompilerServices;



namespace ConsoleSerialPort
{
    class Program
    {
        static void Main(string[] args)
        {
            //CancellationTokenSource cancelTokenSource;
            //CancellationToken token = cancelTokenSource.Token;
            bool isEnabled = false;
            LED.AllOn();

            var httpListener = new HttpListener();
            
            HttpListenAsync();


            var appSettings = ConfigurationManager.AppSettings;

            //Task IzmWorkTask = null;

            var izmDiam = new IzmDiam(
                serialPort: ConfigurationManager.AppSettings.Get("IzmDiamPort"),
                serialSpeed: Int32.Parse(ConfigurationManager.AppSettings.Get("IzmDiamSpeed")) );
            //var izmDiam = new IzmDiam(
            //    serialPort: "COM13",
            //    serialSpeed: Int32.Parse(
            //        ConfigurationManager.AppSettings.Get("IzmDiamSpeed")) );

            var encoder = new Encoder();
            var buttonStartStop = new ButtonStartStop();
            Task taskButtonLisnening = WaitPressButtonAsync();
            Task taskLedBlink = LED.BlinkAsync(color: LedColor.Green);
            ConsoleListen();



            void Start()
            {
                izmDiam.Connect();
                Console.WriteLine("Подключились. Начинаем записывать");
                LED.On(LedColor.Green);
                isEnabled = true;
                WorkAsync();

            }
            void Stop()
            {
                isEnabled = false;
                izmDiam.Disconnect();
                Task TaskLedBlink = LED.BlinkAsync(color: LedColor.Green);
                //if (IzmWorkTask != null) await IzmWorkTask;
                Console.WriteLine("Останавливаем");
                
            }

            async Task WorkAsync()
            {
                int _delayMS = Int32.Parse(ConfigurationManager.AppSettings.Get("ReadDelayMS"));
                int currentLength = 0;
                try
                {
                    var fileData = new FileData();
                    while (isEnabled)
                    {
                        for (int i = 0; (i < fileData.DataCapacity)&&isEnabled; i++)
                        {
                            //string[] dataXY = izmDiam.GetData().Split(' ');

                            //var dataIzm = izmDiam.GetDataDTO();

                            //fileData.Data.Add(new FileData.DataPackage());
                            //fileData.Data[i].DiamX = dataXY[0];
                            //fileData.Data[i].DiamY = dataXY[1];
                            //fileData.Data[i].CurrentTime = DateTime.Now.ToString("O");

                            //string[] dataEncoder = encoder.GetData().Split(' ');
                            //currentLength += Int32.Parse(dataEncoder[0]);
                            //fileData.Data[i].CurrentDistance = currentLength;
                            //fileData.Data[i].CurrentSpeed = dataEncoder[1];

                            var dataIzm = izmDiam.GetDataDTO();

                            fileData.Data.Add(new FileData.DataPackage());
                            fileData.Data[i].DiamX = dataIzm.DiamX;
                            fileData.Data[i].DiamY = dataIzm.DiamY;
                            fileData.Data[i].CurrentTime = DateTime.Now.ToString("O");

                            string[] dataEncoder = encoder.GetData().Split(' ');
                            currentLength += Int32.Parse(dataEncoder[0]);
                            fileData.Data[i].CurrentDistance = currentLength;
                            fileData.Data[i].CurrentSpeed = dataEncoder[1];


#if DEBUG

                            Console.WriteLine("{0,22 } : {1,-8},{2,-8}", "DiamXY:", fileData.Data[i].DiamX, fileData.Data[i].DiamY);
                            //Console.WriteLine("Current Distance-Speed: {0,-8} : {1,8}", fileData.Data[i].CurrentDistance, fileData.Data[i].CurrentSpeed);
#endif

                            await Task.Delay(_delayMS);

                        }
                        //izmDiam.Stop(); //todo:test
                        
                        fileData.SaveToFileAsync();
                        fileData.Data.Clear();
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка разделения данных");
                    Console.WriteLine(ex.Message);
                    LED.On(LedColor.Red);
                }
            }

            async Task WaitPressButtonAsync()
            {
                var currentButtonStat = false;

                while (true)
                {
                    currentButtonStat = buttonStartStop.GetData() == "true" ? true : false;
                    if (currentButtonStat)
                    {
                        if (!isEnabled)
                        {
                            Start();
#if DEBUG
                            Console.WriteLine("buttonStart");
#endif
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("buttonSTOP");
#endif
                            Stop();
                        }
                    }
                    currentButtonStat = false;
                    await Task.Delay(50);
                }
                
            }

            void Exit()
            {
                izmDiam.Disconnect();
                LED.AllOff();
                Environment.Exit(0);
                httpListener.Close();
                httpListener.Stop();
            }

            void ConsoleListen()
            {
                while (true)
                {
                    Console.WriteLine("Ожидание комманды.");
                    string cons = Console.ReadLine();


                    switch (cons)
                    {
                        case "start":
                            Start();
                            break;

                        case "stop":
                            Stop();
                            break;

                        case "exit":
                            Exit();
                            break;

                        case "options":
                            Console.WriteLine("пока не реализовано");
                            break;

                        case "help":
                            Console.WriteLine("start - запуск отслеживания");
                            Console.WriteLine("stop - остановка ослеживания");
                            Console.WriteLine("exit - завершить программу");
                            Console.WriteLine("options - настройки");
                            Console.WriteLine("help - вывод подсказки");
                            break;

                        case "LedOn":
                            LED.AllOn();
                            break;
                        case "LedOff":
                            LED.AllOff();
                            break;
                        case "LedGreen":
                            LED.On(LedColor.Green);
                            break;
                        case "LedYellow":
                            LED.On(LedColor.Yellow);
                            break;
                        case "LedRed":
                            LED.On(LedColor.Red);
                            break;

                        default:
                            Console.WriteLine("Неизвестная комманда. Список комманд: help");
                            break;
                    }
                }
            } 

            async Task HttpListenAsync()
            {
                httpListener.Prefixes.Add("http://10.105.102.49:8080/current/");

                httpListener.Start();

                while (true)
                {
                    var context = await httpListener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    // Обработка GET-запроса
                    if (request.HttpMethod == "GET")
                    {
                        string responseString = "<html><body><h1>Hello, World!</h1></body></html>";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                        Console.WriteLine("Ответ отправлен.");
                    }

                    // Можно добавить дополнительные проверки или логику
                }
            }

        }

    }
    
}

