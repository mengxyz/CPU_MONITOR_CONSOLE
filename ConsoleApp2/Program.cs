using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using Colorful;
using System.Drawing;
using System.Management;
using System.IO.Ports;
using Console = Colorful.Console;

namespace ConsoleApp2
{
    class Program
    {
       public static SerialPort s = new SerialPort();
        public static void RewriteLine(int line, string newtext)
        {
            int currentline = Console.CursorTop;
            Console.SetCursorPosition(0, currentline - line);
            Console.Write(newtext); Console.WriteLine(new string(' ', Console.WindowWidth - newtext.Length));
            Console.SetCursorPosition(0, currentline);
        }
        static void Main(string[] args)
        {/*
            Console.WriteLine("HELLO");
            Console.WriteLine("HELLO");
            RewriteLine(0,"HELLO NEW");
            Console.ReadLine();
            */
            //Console.WriteAsciiAlternating("Hello World", new FrequencyBasedColorAlternator(2, Color.Green, Color.White));
            Console.CursorVisible = true;
            Console.WriteAscii(" CPU MONITOR V1.0 ", Color.FromArgb(255,0,200));
            intro();
            //Console.ReadLine();
            t();
        }

        static void t()
        {
            int i = 0;
            int st = 6;
            Console.Clear();
            Console.WriteAscii(" CPU MONITOR V1.0 ", Color.FromArgb(255, 0, 200));
            Console.SetCursorPosition(0, 5);
            Console.WriteLine("  CPU TEMP",Color.Red);
            while (true)
            {
                Computer myComputer = new Computer();
                myComputer.CPUEnabled = true;
                myComputer.Open();
                List<int> ctemp_value = new List<int>();

                foreach (var hardwareItem in myComputer.Hardware)
                {

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.Identifier.ToString() == "/intelcpu/0/temperature/0" || sensor.Identifier.ToString() == "/intelcpu/0/temperature/1" || sensor.Identifier.ToString() == "/intelcpu/0/temperature/2" || sensor.Identifier.ToString() == "/intelcpu/0/temperature/3")
                        {
                            string name = sensor.Identifier.ToString();
                            name = name.Replace("/intelcpu/0/temperature/", "CPU ");
                            ctemp_value.Add(Convert.ToInt32(sensor.Value));
                        }

                    }
                }//COLOR
                int r = 255;
                int g = 0;
                int b = 255;

                double avg = ctemp_value.Average();
                //print cpu
                Console.SetCursorPosition(0, st);
                Console.Write("\r  CPU 0 : {0}%   ", ctemp_value[0], Color.FromArgb(r, g, b));
                Console.SetCursorPosition(0, st + 1);
                Console.Write("\r  CPU 1 : {0}%   ", ctemp_value[1], Color.FromArgb(r, g, b));
                Console.SetCursorPosition(0, st + 2);
                Console.Write("\r  CPU 2 : {0}%   ", ctemp_value[2], Color.FromArgb(r, g, b));
                Console.SetCursorPosition(0, st + 3);
                Console.Write("\r  CPU 3 : {0}%   ", ctemp_value[3], Color.FromArgb(r, g, b));
                Console.SetCursorPosition(0, st + 4);
                Console.Write("\r  AVG {0}%   ", avg , Color.FromArgb(r, g, b));
                //SEt DELEY
                i += 1;
                if(i % 2 == 0)
                {
                    s.Write("1");
                }
                else
                {
                    s.Write("2");
                }
                Thread.Sleep(300);
            }
        }
        static void intro()
        {
            List<string> port_na = new List<string>();
            string[] portNames = SerialPort.GetPortNames();
            for (int y = 0; y < portNames.Length; y++)
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT *  FROM Win32_PnPEntity WHERE Name LIKE '%COM%'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["Caption"].ToString().Contains("(COM"))
                    {
                        port_na.Add(queryObj["Caption"].ToString());    
                    }

                }
            }
            Console.SetCursorPosition(0, 5);
            Console.WriteLine("  PORT AVILABLE \n", Color.LightGreen);
            int i = 0;
            Random rnd = new Random();
            port_na = port_na.Distinct<string>().ToList<string>();
            foreach (string x in port_na)
            {
                i += 1;
                Console.WriteLine("  [{0}]  {1} ", i,x,Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
            }
            Console.Write("  SELECT PORT : ", Color.OrangeRed);
            int input = Convert.ToInt32(Console.ReadLine());
            string port = getBetween(port_na[input - 1].ToString(),"(",")");
            s = new SerialPort(port, 9600);
            s.Open();
            Console.WriteLine("  OPEN => : {0} ", port_na[input - 1].ToString(), Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
            Console.ReadLine();
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }
}
