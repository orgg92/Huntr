namespace Radar.Services
{
    using Radar.Common;
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

        private HostTracker hostTracker;

        private List<Host> ActiveHosts = new List<Host>();
        private List<AbstractHost> hostList = new List<AbstractHost>();

        private readonly IIPManipulationService _iPManipulationService;

        public NetworkScanner(IIPManipulationService iPManipulationService)
        {
            _subnetList = new SubnetsList();
            _iPManipulationService = iPManipulationService;

            ifaces = NetworkInterface.GetAllNetworkInterfaces();
            hostTracker = new HostTracker();
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
                ConsoleTools.WriteToConsole($"({i}) {iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Red);

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

                ConsoleTools.WriteToConsole($"{iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Red);

                return subnetMask.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask) // host IP and subnet
        {
            // Calculate number of hosts from subnet mask
            var subnet = _subnetList.ReturnSubnetInfo(subnetMask);

            var segment = new IPSegment(ipAddress.ToString(), subnet.SubnetMask);

            // Calculate first IP to scan based on input IP
            firstHost = segment.Hosts().First().ToIpString();
            lastHost = segment.Hosts().Last().ToIpString();
            targetIp = firstHost;

            // Create list of threads 
            var threadList = new List<Thread>();

            var numberOfThreads = Process.GetCurrentProcess().Threads.Count;

            // inefficient but without this list, the multithreading process can skip hosts 
            for (int i = 0; i < subnet.NumberOfHosts; i++)
            {
                hostList.Add(new AbstractHost { IP = targetIp });
                this.targetIp = IncrementIpAddress(targetIp.ToString());
            }

            for (int i = 0; i < subnet.NumberOfHosts; i = i++)
            {

                // Make host scanning quicker by multithreading 
                for (int t = 0; t < numberOfThreads; t++)
                {

                    if (targetIp != lastHost)
                    {
                        threadList.Add(new Thread(() => ThreadedPingRequest()));
                        threadList[t].Start();
                        Thread.Sleep(150);
                    }
                }

                // Increment by number of threads 
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

        private void ResolveHostNames()
        {
            //this.ActiveHosts.ForEach(x => x.HostName = QueryDNS(x));

        }

        private IPHostEntry QueryDNS(Host host)
        {
            try
            {
                return Dns.GetHostByAddress(host.IP);
            }
            catch (Exception e)
            {
                return new IPHostEntry();
            }

        }


        private string FormatStopwatch(Stopwatch stopwatch)
        {
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
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
            var foundHosts = new List<string>();

            Host host;

            // Block needs testing on Linux to confirm that ArpScan will work in the same way as the Windows 

            //var ping = new Ping();
            //int timeout = 100;
            //var reply = ping.Send(targetIp, timeout);
            //var result = reply.Status;
            //if (result == IPStatus.Success)
            //{
            //    foundHosts.Add(targetIp.ToString());
            //}

            // Scan host to get MAC address
            host = ArpScan.Scan(targetIp.ToString());

            // All properties are null if ARP scan fails
            if (host.IP is not null)
            {
                //ConsoleTools.WriteToConsole($"Found host: {host.IP} | {host.MAC} | {host.Vendor}", ConsoleColor.Green);
                ConsoleTools.WriteToConsole($"Found host: {host.IP}", ConsoleColor.Green);
                host.HostName = QueryDNS(host).HostName ?? "Unknown";
                ActiveHosts.Add(host);

            }

            return true;

        }
    }
}
