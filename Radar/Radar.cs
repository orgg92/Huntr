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

        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner, IHostToolsService hostToolsService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            HostTools = new HostTools();
        }

        public void StartApp()
        {
            StartScan();
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

