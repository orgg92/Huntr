namespace Radar.Common.HostTools
{
    using Radar.Common.Config;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;

    public class PortScanner : HostTool
    {
        private readonly ILoggingService _loggingService;

        private List<PortInfo> openPorts;
        private List<PortInfo> targetPorts;

        public readonly static string portInfoPath = Config.PORT_LIST_PATH;

        public PortScanner(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            openPorts = new List<PortInfo>();
            targetPorts = new List<PortInfo>();

            this.Name = "PortScanner";
        }

        public bool CheckHost(string ipAddress)
        {
            if (!Config.CUSTOM_PORT_SCAN)
            {
                CreatePortList();
            }
            else
            {
                LoadPortList();
            }

            var threadList = new List<Thread>();
            var numberOfThreads = Process.GetCurrentProcess().Threads.Count;

            var length = targetPorts.Any(x => !x.Attempted);

            for (int i = 0; i < targetPorts.Count(); i++)
            {
                for (int t = 0; t < numberOfThreads; t++)
                {

                    if (i + t < targetPorts.Count())
                    {
                        threadList.Add(new Thread(() => ThreadedPortRequest(ipAddress, targetPorts[i + t])));
                        threadList[t].Start();
                        Thread.Sleep(150);
                    }

                }

                threadList.WaitAll();
                threadList.Clear();

                i = i + numberOfThreads;
            }

            if (openPorts.Any())
            {
                _loggingService.DisplayPortList(openPorts);
            }

            return true;
        }

        private void LoadPortList()
        {
            var ports = CommonOperations.LoadListFromFile(portInfoPath);

            foreach (var portInfo in ports)
            {
                targetPorts.Add(new PortInfo
                {
                    PortNum = int.Parse(portInfo.Split(CommonConsole.separator[0])[0]),
                    PortName = portInfo.Split(CommonConsole.separator[0])[1]
                });
            }
        }

        private void CreatePortList()
        {
            if (Config.CUSTOM_PORT_SCAN && Config.CUSTOM_PORTS.Any())
            {
                foreach(var port in Config.CUSTOM_PORTS)
                {
                    targetPorts.Add(new PortInfo { PortNum = port });
                }
            }
        }

        private void ThreadedPortRequest(string ipAddress, PortInfo portInfo)
        {
            portInfo.Attempted = true;

            if (targetPorts.Any(x => !x.Attempted))
            {
                using TcpClient tcpClient = new TcpClient();
                CommonConsole.WriteToConsole($"Trying port {portInfo.PortNum}", ConsoleColor.Yellow);

                try
                {
                    tcpClient.Connect(IPAddress.Parse(ipAddress), portInfo.PortNum);

                    openPorts.Add(portInfo);

                    CommonConsole.WriteToConsole($"Port {portInfo.PortNum} open", ConsoleColor.Green);

                }
                catch (Exception e)
                {
                    Console.ResetColor();
                    CommonConsole.WriteToConsole($"Port {portInfo.PortNum} closed", ConsoleColor.Red);

                }
            }

        }
    }
}
