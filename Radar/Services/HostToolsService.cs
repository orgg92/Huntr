namespace Radar.Services
{
    using Radar.Common;
    using Radar.Common.Config;
    using Radar.Common.HostTools;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services.Interfaces;

    public class HostToolsService : IHostToolsService
    {
        private readonly ILoggingService _loggingService;

        private PortScanner PortScanner;

        private const string IP = "IP",
                             MAC = "MAC",
                             Vendor = "Vendor",
                             No = "#";

        private const int IP_Length = 15,
                          Vendor_Length = 64;



        public HostToolsService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            PortScanner = new PortScanner(_loggingService);
        }

        public void ChooseService(IEnumerable<Host> hosts)
        {
            var selectedHost = HostSelect(hosts);
            PortScanner.CheckHost(selectedHost.IP);

        }

        private Host HostSelect(IEnumerable<Host> hosts)
        {
            ConsoleTools.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            return hosts.ElementAt(selectedHost);
        }

        private void ToolSelector()
        {
            ConsoleTools.WriteToConsole("Select a tool...", ConsoleColor.Yellow);
            var input = int.Parse(Console.ReadLine()) - 1;


        }
    }
}
