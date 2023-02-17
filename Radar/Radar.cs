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
        private IEnumerable<Host> Hosts;

        public string[] CommandOptions = new string[] { "Network Scan", "DHCP Finder", "DNS Finder" };
        public const string FeatureSelection = "Select one of the following options...";

        public RadarScanner(IIPManipulationService iPManipulationService, INetworkScanner networkScanner, IHostToolsService hostToolsService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            HostTools = new HostTools();
        }

        public void StartApp()
        {
            //Console.WriteLine(FeatureSelection);

            //for (int i = 0; i < CommandOptions.Length; i++)
            //{
            //    Console.WriteLine($"({i + 1}) {CommandOptions[i]}");
            //}

            //var input = Console.ReadLine();
            //switch (int.Parse(input))
            //{
            //    case 1:
                    StartScan();
            //        break;
            //    case 2:
            //        // Find DHCP
            //        break;
            //    case 3:
            //        // Find DNS
            //        break;
            //}

            _hostToolsService.ChooseService(Hosts);
        }

        public void StartScan()
        {
            Hosts = Enumerable.Empty<Host>();

            var iface = _networkScanner.FindInterfaces();
            Hosts = _networkScanner.StartScan(iface);
        }

        private static void SaveToLogFile()
        {

        }
    }
}

