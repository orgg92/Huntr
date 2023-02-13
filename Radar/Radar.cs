﻿using System;
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

namespace Radar
{
    public class RadarScanner
    {
        private readonly INetworkScanner _networkScanner;
        private readonly IHostToolsService _hostToolsService;
        private HostTools HostTools { get; set; }


        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner, IHostToolsService hostToolsService)
        {
            _networkScanner = networkScanner;
            HostTools = new HostTools();
            _hostToolsService = hostToolsService;
        }

        public async Task StartScan()
        {
            Console.WriteLine("Run network scan? [Y/N]");
            var input = Console.ReadLine();


            var hosts = Enumerable.Empty<Host>();

            if (input.ToLower() == "Y".ToLower())
            {
                var iface = _networkScanner.FindInterfaces();   
                hosts = _networkScanner.StartScan(iface);
            }

            _hostToolsService.ChooseService(hosts);

            



        }

        private static void SaveToLogFile()
        {

        }


    }

}
