namespace Radar.Common.HostTools
{
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;

    public class PortScanner : HostTool
    {
        private readonly ILoggingService _loggingService;

        private List<PortInfo> openPorts;

        private List<PortInfo> commonPorts;

        public readonly static string path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Resources/";
        public readonly static string portInfoPath = $"{path}\\PortList.txt";

        public PortScanner(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            openPorts = new List<PortInfo>();
            commonPorts = new List<PortInfo>();

            this.Name = "PortScanner";
        }

        public bool CheckHost(string ipAddress)
        {

            GetPorts();

            // Create list of threads 
            var threadList = new List<Thread>();
            var numberOfThreads = Process.GetCurrentProcess().Threads.Count;

            var length = commonPorts.Any(x => !x.Attempted);

            for (int i = 0; i < commonPorts.Count(); i++)
            {
                // Make host scanning quicker by multithreading 
                for (int t = 0; t < numberOfThreads; t++)
                {

                    if (i + t < commonPorts.Count())
                    {
                        threadList.Add(new Thread(() => ThreadedPortRequest(ipAddress, commonPorts[i + t])));
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

        private void GetPorts()
        {
            var ports = FileReader.LoadListFromFile(portInfoPath);

            foreach (var portInfo in ports)
            {
                commonPorts.Add(new PortInfo {
                    PortNum = int.Parse(portInfo.Split(CommonConsole.separator[0])[0]),
                    PortName = portInfo.Split(CommonConsole.separator[0])[1] });
            }

        }

        private void ThreadedPortRequest(string ipAddress, PortInfo portInfo)
        {
            portInfo.Attempted = true;

            if (commonPorts.Any(x => !x.Attempted))
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    ConsoleTools.WriteToConsole($"Trying port {portInfo.PortNum}", ConsoleColor.Yellow);

                    try
                    {
                        tcpClient.Connect(IPAddress.Parse(ipAddress), portInfo.PortNum);

                        openPorts.Add(portInfo);

                        ConsoleTools.WriteToConsole($"Port {portInfo.PortNum} open", ConsoleColor.Green);

                    }
                    catch (Exception e)
                    {
                        ConsoleTools.WriteToConsole($"Port {portInfo.PortNum} closed", ConsoleColor.Red);

                    }
                }
            }

        }
    }
}
