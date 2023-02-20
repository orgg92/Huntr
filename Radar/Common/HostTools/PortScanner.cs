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

            var ports = Config.Config.Ports;

            for (int i = 0; i < commonPorts.Count(); i++)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    ConsoleTools.WriteToConsole($"Trying port {commonPorts[i].PortNum}", ConsoleColor.Yellow);

                    try
                    {
                        tcpClient.Connect(IPAddress.Parse(ipAddress), commonPorts[i].PortNum);

                        openPorts.Add(openPorts[i]);
                        
                        ConsoleTools.WriteToConsole($"Port {commonPorts[i].PortNum} open", ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        ConsoleTools.WriteToConsole($"Port {commonPorts[i].PortNum} closed", ConsoleColor.Red);

                    }
                }

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

        private void ThreadedPortRequest()
        {

        }


    }
}
