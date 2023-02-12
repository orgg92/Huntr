using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Radar.Services;

namespace Radar
{
    public class RadarScanner
    {
        private readonly INetworkScanner _networkScanner;


        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner)
        {
            _networkScanner = networkScanner;
        }

        public async Task StartScan()
        {
            var iface = _networkScanner.FindInterfaces();
            _networkScanner.ScanInterfaces(iface);



        }

        private static void SaveToLogFile()
        {

        }


    }

}
