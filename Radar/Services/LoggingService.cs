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

    public class LoggingService : ILoggingService
    {
        private string configPath = $"{System.Reflection.Assembly.GetExecutingAssembly().Location}/Logs/log.txt";

        public LoggingService()
        {

        }

        public void LogToConsole(string message, ConsoleColor color)
        {
            ConsoleTools.WriteToConsole(message, color);
        }

        public void LogToFile(string message)
        {
            // ToDo
        }

        public void DisplayHostList(IEnumerable<Host> hosts)
        {

            ConsoleTools.WriteToConsole(CommonConsole.TableHeader, ConsoleColor.Red);

            for (int i = 0; i < hosts.Count(); i++)
            {
                // create host for display to console
                var host = hosts.Select(x => new Host() { HostName = x.HostName, MAC = x.MAC, IP = x.IP, Vendor = x.Vendor }).ElementAt(i);
                var paddedHost = StringTableFormatter.PadPropertiesForDisplay(host, CommonConsole.TableHeaderMessages[4]);

                ConsoleTools.WriteToConsole(
                    String.Format(
                        "{0,3} |{1}| {2,5} |{3}| {4} |",
                        i + 1,
                        paddedHost.IP,
                        paddedHost.MAC,
                        paddedHost.Vendor,
                        paddedHost.HostName),
                ConsoleColor.Red);

            }
        }
    }
}
