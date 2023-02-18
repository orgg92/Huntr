using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Radar.Common.HostTools;
using Radar.Services.Interfaces;
using Radar.Common.NetworkModels;
using Radar.Common.Util;

namespace Radar
{
    public class RadarScanner
    {
        private readonly INetworkScanner _networkScanner;
        private readonly IHostToolsService _hostToolsService;
        private HostTools HostTools { get; set; }
        private IEnumerable<Host> Hosts;

        public string[] CommandOptions = new string[] { "Network Scan"};
        public const string FeatureSelection = "Select one of the following options...";

        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner, IHostToolsService hostToolsService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            HostTools = new HostTools();
        }

        public void StartApp()
        {
            Console.WriteLine(FeatureSelection);

                for (int i = 0; i < CommandOptions.Length; i++)
                {
                    ConsoleTools.WriteToConsole($"({i + 1}) {CommandOptions[i]}", ConsoleColor.Yellow);
                }

            var input = Console.ReadLine();

            bool successfullyParsed = int.TryParse(input, out var ignored);

            if (successfullyParsed)
            {
                switch (int.Parse(input))
                {
                    case 1:
                        StartScan();
                        break;

                    default:
                        ConsoleTools.WriteToConsole("Invalid selection", ConsoleColor.Red);
                        break;
                }

                _hostToolsService.ChooseService(Hosts);
            } else
            {
                InvalidSelection();
            }

        }

        public void StartScan()
        {
            Hosts = Enumerable.Empty<Host>();

            var iface = _networkScanner.FindInterfaces();
            Hosts = _networkScanner.StartScan(iface);
        }

        public void InvalidSelection()
        {
            ConsoleTools.WriteToConsole("Invalid selection", ConsoleColor.Red);
        }

    }
}

