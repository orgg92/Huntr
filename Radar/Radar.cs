namespace Radar
{
    using Radar.Common;
    using Radar.Common.HostTools;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services.Interfaces;

    public class RadarScanner
    {
        private readonly INetworkScanner _networkScanner;
        private readonly IHostToolsService _hostToolsService;
        private HostTools HostTools { get; set; }
        private IEnumerable<Host> Hosts;

        public string[] CommandOptions = new string[] { "Network Scan" };
        public const string FeatureSelection = "Select one of the following options...";

        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner, IHostToolsService hostToolsService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            HostTools = new HostTools();
        }

        public void StartApp()
        {
            StartScan();
            _hostToolsService.ChooseService(Hosts);
        }

        public void StartScan()
        {
            Hosts = Enumerable.Empty<Host>();

        Input:
            var ifaces = _networkScanner.FindInterfaces();

            ConsoleTools.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
            ConsoleTools.WriteToConsole("Select an interface to scan on... (Default: ALL - this may take a while)", ConsoleColor.Yellow);

            var input = Console.ReadLine();

            // not ideal but prevents any other selections
            if (!ValidateInput(input, ifaces.Length))
            {
                InvalidSelection();
                goto Input;
            }
            else
            {
                Hosts = _networkScanner.StartScan(input);
            }
        }

        public void InvalidSelection()
        {
            ConsoleTools.WriteToConsole("Invalid selection", ConsoleColor.Red);
        }

        public bool ValidateInput(string input, int interfaceCount)
        {
            if (int.TryParse(input, out var parsedInput))
            {
                if (parsedInput < interfaceCount)
                    return true;

                return false;
            }
            else
            {
                return false;
            }
        }

    }
}

