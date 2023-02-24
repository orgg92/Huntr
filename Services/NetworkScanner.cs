﻿namespace Radar.Services
{
    using Radar.Common.Config;
    using Radar.Common.Netscan;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services.Interfaces;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public class NetworkScanner : INetworkScanner
    {
        private string foundInterfacesMsg = "Found {0} interfaces...",
                       findingInterfacesMsg = "Searching for network interfaces...",
                       findingNetworkHostsMsg = "Searching for hosts...",
                       firstHost = String.Empty,
                       lastHost = String.Empty;

        private SubnetsList _subnetList;
        private int interfaceCount;
        private int threadCount;

        private NetworkInterface[] ifaces;
        private UnicastIPAddressInformationCollection ipAddresses;
        private IPAddress ipAddress;
        private string subnetMask;
        private string targetIp;
        private Stopwatch stopWatch;

        private List<Host> ActiveHosts = new List<Host>();
        private List<AbstractHost> hostList = new List<AbstractHost>();
        private List<Thread> threadList = new List<Thread>();


        private readonly IIPManipulationService _iPManipulationService;

        public NetworkScanner(IIPManipulationService iPManipulationService)
        {
            _subnetList = new SubnetsList();
            _iPManipulationService = iPManipulationService;

            ifaces = NetworkInterface.GetAllNetworkInterfaces();
            stopWatch = new Stopwatch();
        }

        public NetworkInterface[] FindInterfaces()
        {
            interfaceCount = ifaces.Count();

            CommonConsole.WriteToConsole(findingInterfacesMsg, ConsoleColor.Yellow);
            CommonConsole.WriteToConsole(foundInterfacesMsg.Replace("{0}", interfaceCount.ToString()), ConsoleColor.Yellow);
            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            int i = 1;

            foreach (var iface in ifaces)
            {
                ipAddresses = iface.GetIPProperties().UnicastAddresses;

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First();
                subnetMask = _iPManipulationService.ReturnSubnetmask(ipAddress);
                CommonConsole.WriteToConsole($"({i}) {iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Green);

                var bytes = ipAddress.GetAddressBytes();
                var binarySubnetMask = String.Join(".", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                int mask = binarySubnetMask.Count(b => b == '1');
                i++;
            }

            return ifaces;
        }

        public IEnumerable<Host> StartScan(string userInput)
        {
            stopWatch.Start();

            CommonConsole.WriteToConsole(findingNetworkHostsMsg, ConsoleColor.Yellow);

            subnetMask = ScanInterfaces(userInput);
            var hosts = ScanNetwork(ipAddress, subnetMask.ToString());

            return ActiveHosts.Select(x => x).Distinct().ToArray();
        }

        public string ScanInterfaces(string userInput)
        {
            var iface = ifaces[int.Parse(userInput) - 1];

            try
            {
                ipAddresses = iface.GetIPProperties().UnicastAddresses;
                var ipv4Mask = ipAddresses.Where(x => x.IPv4Mask.ToString() != "0.0.0.0").Select(x => x.IPv4Mask).First();

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First();

                CommonConsole.WriteToConsole($"Selected: {iface.Name} on {ipAddress}/{ipv4Mask} ", ConsoleColor.Green);

                return ipv4Mask.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask)
        {
            threadCount = Process.GetCurrentProcess().Threads.Count;

            if (Config.CUSTOM_IP_SCAN)
            {
                ExecuteCustomScan();
            } else
            {
                ExecuteFullScan();
            }

            stopWatch.Stop();

            var elapsedTime = FormatStopwatch(stopWatch);

            CommonConsole.WriteToConsole($"Found {ActiveHosts.Count()} hosts...", ConsoleColor.Yellow);
            CommonConsole.WriteToConsole($"Scan completed in: {elapsedTime}", ConsoleColor.Green);

            return ActiveHosts;

        }

        private IPHostEntry QueryDNS(Host host)
        {
            try
            {
                return Dns.GetHostEntry(host.IP);
            }
            catch (Exception e)
            {
                return new IPHostEntry();
            }

        }

        private void ExecuteFullScan()
        {
            var subnet = _subnetList.ReturnSubnetInfo(subnetMask);
            var segment = new IPSegment(ipAddress.ToString(), subnet.SubnetMask);

            firstHost = segment.Hosts().First().ToIpString();
            lastHost = segment.Hosts().Last().ToIpString();
            targetIp = firstHost;

            for (int i = 0; i < subnet.NumberOfHosts; i++)
            {
                hostList.Add(new AbstractHost { IP = targetIp });
                this.targetIp = IncrementIpAddress(targetIp.ToString());
            }

            ThreadedPingRequest(subnet.NumberOfHosts);


        }

        private void ExecuteCustomScan()
        {
            foreach (var ip in Config.CUSTOM_IP_ADDRESSES)
            {
                hostList.Add(new AbstractHost { IP = ip });
            }

            ThreadedPingRequest(Config.CUSTOM_IP_ADDRESSES.Count());
        }

        private void ThreadedPingRequest(int loopCount)
        {
            for (int i = 0; i < loopCount; i = i++)
            {
                for (int t = 0; t < threadCount; t++)
                {
                    if (targetIp != lastHost)
                    {
                        threadList.Add(new Thread(() => PingRequest()));
                        threadList[t].Start();
                        Thread.Sleep(200);
                    }
                }

                i = i + threadCount;
                threadList.WaitAll();
                threadList.Clear();
            }
        }

        public void PingRequest()
        {

            if (hostList.Any(x => x.PingAttempted is false))
            {
                var targetHost = hostList.Select(x => x).Where(x => x.PingAttempted is false).First();
                targetIp = targetHost.IP.ToString();
                hostList.Remove(targetHost);
                CommonConsole.WriteToConsole($"Trying host: {targetIp}", ConsoleColor.Yellow);

                var result = PingHost(IPAddress.Parse(targetIp));
            }
        }

        public string IncrementIpAddress(string ipAddress)
        {
            var ipSplit = Convert.ToString(ipAddress).Split(".");

            // Increment the last octet
            ipSplit[ipSplit.Count() - 1] = (int.Parse(ipSplit[ipSplit.Count() - 1]) + 1).ToString();

            for (int i = 3; i > 0; i--)
            {
                // check if any octet is 256 (if so - increment previous octet and reset current octet to 0, but only for the fourth, third and second octets)
                if (ipSplit[i] == "256")
                {
                    if (i > 1)
                    {
                        ipSplit[i - 1] = (int.Parse(ipSplit[i - 1]) + 1).ToString();
                        ipSplit[i] = "0";
                    }
                }
            }

            return String.Join(".", ipSplit);
        }

        public bool PingHost(IPAddress targetIp)
        {
            Host host;

            host = ArpScan.Scan(targetIp.ToString());

            if (host.IP is not null)
            {
                CommonConsole.WriteToConsole($"Found host: {host.IP}", ConsoleColor.Green);
                host.HostName = QueryDNS(host).HostName ?? "Unknown";
                ActiveHosts.Add(host);
            }

            return true;
        }

        private string FormatStopwatch(Stopwatch stopwatch)
        {
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return elapsedTime;
        }
    }
}
