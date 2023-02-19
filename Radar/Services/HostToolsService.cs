namespace Radar.Services
{
    using Radar.Common;
    using Radar.Common.HostTools;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services.Interfaces;

    public class HostToolsService : IHostToolsService
    {
        private readonly ILoggingService _loggingService;

        private const string IP = "IP",
                             MAC = "MAC",
                             Vendor = "Vendor",
                             No = "#";

        private const int IP_Length = 15,
                          Vendor_Length = 64;


        private Flooder Flooder { get; set; }

        public HostToolsService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            CommonConsole.TableHeader = $"{CommonConsole.TableHeaderMessages[0]}|{CommonConsole.TableHeaderMessages[1]}|{CommonConsole.TableHeaderMessages[2]}|{CommonConsole.TableHeaderMessages[3]}|{CommonConsole.TableHeaderMessages[4]}|";
        }

        public void ChooseService(IEnumerable<Host> hosts)
        {
            try
            {
                var targetHost = new Host();

                if (hosts.Count() == 0)
                {
                    ConsoleTools.WriteToConsole("No hosts detected... Enter target IP", ConsoleColor.Red);
                    var targetIP = Console.ReadLine();

                    targetHost.IP = targetIP;

                }
                else
                {
                    _loggingService.DisplayHostList(hosts);
                    LoggingPrompt();


                }

                ConsoleTools.WriteToConsole("Select a tool...", ConsoleColor.Yellow);
                var input = int.Parse(Console.ReadLine()) - 1;
            }
            catch (Exception e)
            {

            }
        }

        private void LoggingPrompt()
        {
            ConsoleTools.WriteToConsole("Write to logfile? [Y/N]", ConsoleColor.Yellow);
            var logging = Console.ReadLine();

            if (logging.ToLower() == "y")
            {
                ConsoleTools.WriteToConsole("Writing to logfile...", ConsoleColor.Yellow);
            }
            else if (logging.ToLower() == "n")
            {
                ConsoleTools.WriteToConsole("Skipping log file", ConsoleColor.Yellow);
            }
            else
            {
                ConsoleTools.WriteToConsole(CommonConsole.InvalidSelection, ConsoleColor.Red);
            }
        }

        private Host HostSelect(IEnumerable<Host> hosts)
        {
            ConsoleTools.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            return hosts.ElementAt(selectedHost);
        }
    }
}
