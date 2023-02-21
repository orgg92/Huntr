namespace Radar.Services
{
    using Radar.Common.NetworkModels;
    using Radar.Common;
    using Radar.Common.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Common.Util.ConsoleTools;
    using System.IO;
    using Radar.Common.HostTools;

    public class LoggingService : ILoggingService
    {
        public readonly static string configPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs";
        public readonly static string logPath = $"{configPath}\\log.txt";

        public LoggingService()
        {
            CommonConsole.TableHeader = $"{CommonConsole.DeviceTableHeaderMessages[0]}|{CommonConsole.DeviceTableHeaderMessages[1]}|{CommonConsole.DeviceTableHeaderMessages[2]}|{CommonConsole.DeviceTableHeaderMessages[3]}|{CommonConsole.DeviceTableHeaderMessages[4]}|";
        }

        public void LogToConsole(string message, ConsoleColor color)
        {
            ConsoleTools.WriteToConsole(message, color);
        }

        public void LogToFile(string[] textArray)
        {
            // ToDo

            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            using (FileStream fs = File.OpenWrite(logPath))
            {
                byte[] buffer;

                foreach (var str in CommonConsole.DeviceTableHeaderMessages)
                {
                    buffer = ConvertStringToBytes(str + "|");
                    fs.Write(buffer, 0, buffer.Length);
                }

                buffer = ConvertStringToBytes("\r\n");
                fs.Write(buffer, 0, buffer.Length);

                foreach(var str in textArray)
                {
                    buffer = ConvertStringToBytes(str + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private byte[] ConvertStringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public string[] DisplayHostList(IEnumerable<Host> hosts)
        {

            ConsoleTools.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            ConsoleTools.WriteToConsole(CommonConsole.TableHeader, ConsoleColor.Red);

            var formattedTextArray = new string[hosts.Count()];

            for (int i = 0; i < hosts.Count(); i++)
            {
                // create host for display to console
                var host = hosts.Select(x => new Host() { HostName = x.HostName, MAC = x.MAC, IP = x.IP, Vendor = x.Vendor }).ElementAt(i);
                var paddedHost = StringTableFormatter.PadPropertiesForDisplay(host, CommonConsole.DeviceTableHeaderMessages[4]);

                var formattedText = String.Format(
                        "{0,3} |{1}| {2,5} |{3}| {4} |",
                        i + 1,
                        paddedHost.IP,
                        paddedHost.MAC,
                        paddedHost.Vendor,
                        paddedHost.HostName);

                ConsoleTools.WriteToConsole(formattedText, ConsoleColor.Red);

                formattedTextArray[i] = formattedText;

            }

            return formattedTextArray;
        }

        public void DisplayPortList(List<PortInfo> openPorts)
        {
            ConsoleTools.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            var formattedText = String.Format("|{0} | {1} |", CommonConsole.PortTableHeaderMessages[0], CommonConsole.PortTableHeaderMessages[1]);
            ConsoleTools.WriteToConsole(formattedText, ConsoleColor.Yellow);

            foreach(var port in openPorts)
            {
                var textString = String.Format("| {0,-10} | {1,-43} |", port.PortNum, port.PortName);
                ConsoleTools.WriteToConsole(textString, ConsoleColor.Yellow);
            }

        }
    }
}
