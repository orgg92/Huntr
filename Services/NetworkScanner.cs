namespace Radar.Services
{
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

        private NetworkInterface[] ifaces;
        private UnicastIPAddressInformationCollection ipAddresses;
        private IPAddress ipAddress;
        private string subnetMask;
        private string targetIp;
        private Stopwatch stopWatch;

        private List<Host> ActiveHosts = new List<Host>();
        private List<AbstractHost> hostList = new List<AbstractHost>();

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

            ConsoleTools.WriteToConsole(findingInterfacesMsg, ConsoleColor.Yellow);
            ConsoleTools.WriteToConsole(foundInterfacesMsg.Replace("{0}", interfaceCount.ToString()), ConsoleColor.Yellow);

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
                ConsoleTools.WriteToConsole($"({i}) {iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Green);

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

            ConsoleTools.WriteToConsole(findingNetworkHostsMsg, ConsoleColor.Yellow);

            var subnetMask = ScanInterfaces(userInput);
            var hosts = ScanNetwork(ipAddress, subnetMask.ToString());

            return ActiveHosts.Select(x => x).Distinct().ToArray();
        }

        public string ScanInterfaces(string userInput)
        {
            var iface = ifaces[int.Parse(userInput) - 1];

            try
            {

                ipAddresses = iface.GetIPProperties().UnicastAddresses;
                var subnetMask = ipAddresses.Where(x => x.IPv4Mask.ToString() != "0.0.0.0").Select(x => x.IPv4Mask).First();

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First();

                ConsoleTools.WriteToConsole($"Selected: {iface.Name} on {ipAddress}/{subnetMask} ", ConsoleColor.Green);

                return subnetMask.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask)
        {
            var subnet = _subnetList.ReturnSubnetInfo(subnetMask);
            var segment = new IPSegment(ipAddress.ToString(), subnet.SubnetMask);

            firstHost = segment.Hosts().First().ToIpString();
            lastHost = segment.Hosts().Last().ToIpString();
            targetIp = firstHost;

            var threadList = new List<Thread>();
            var numberOfThreads = Process.GetCurrentProcess().Threads.Count;

            for (int i = 0; i < subnet.NumberOfHosts; i++)
            {
                hostList.Add(new AbstractHost { IP = targetIp });
                this.targetIp = IncrementIpAddress(targetIp.ToString());
            }

            for (int i = 0; i < subnet.NumberOfHosts; i = i++)
            {
                for (int t = 0; t < numberOfThreads; t++)
                {
                    if (targetIp != lastHost)
                    {
                        threadList.Add(new Thread(() => ThreadedPingRequest()));
                        threadList[t].Start();
                        Thread.Sleep(150);
                    }
                }

                i = i + numberOfThreads;
                threadList.WaitAll();
                threadList.Clear();
            }

            stopWatch.Stop();

            var elapsedTime = FormatStopwatch(stopWatch);

            ConsoleTools.WriteToConsole($"Found {ActiveHosts.Count()} hosts...", ConsoleColor.Yellow);
            ConsoleTools.WriteToConsole($"Scan completed in: {elapsedTime}", ConsoleColor.Green);

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


        private string FormatStopwatch(Stopwatch stopwatch)
        {
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return elapsedTime;
        }

        public void ThreadedPingRequest()
        {
            if (hostList.Any(x => x.PingAttempted is false))
            {
                var targetHost = hostList.Select(x => x).Where(x => x.PingAttempted is false).First();
                targetIp = targetHost.IP.ToString();
                hostList.Remove(targetHost);
                ConsoleTools.WriteToConsole($"Trying host: {targetIp}", ConsoleColor.Yellow);

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
                ConsoleTools.WriteToConsole($"Found host: {host.IP}", ConsoleColor.Green);
                host.HostName = QueryDNS(host).HostName ?? "Unknown";
                ActiveHosts.Add(host);
            }

            return true;
        }
    }
}
