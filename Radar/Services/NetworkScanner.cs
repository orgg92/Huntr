using Radar.Common;
using Radar.Common.NetworkModels;
using Radar.Common.Util;
using Radar.Services.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Radar.Services
{


    public class NetworkScanner : INetworkScanner
    {
        private const string spacer = "***********************************************************";

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

        }

        public string FindInterfaces()
        {
            try
            {
                interfaceCount = ifaces.Count();

                WriteToConsole(findingInterfacesMsg, ConsoleColor.Yellow);
                WriteToConsole(foundInterfacesMsg.Replace("{0}", interfaceCount.ToString()), ConsoleColor.Yellow);

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
                    WriteToConsole($"({i}) {iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Red);

                    var bytes = ipAddress.GetAddressBytes();
                    var binarySubnetMask = String.Join(".", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                    int mask = binarySubnetMask.Count(b => b == '1');
                    i++;
                }

                Console.WriteLine(spacer);

                WriteToConsole("Select an interface to scan on... (Default: ALL - this may take a while)", ConsoleColor.Yellow);

                var input = Console.ReadLine();

                var hosts = new Host[120000];

                return input;

            } catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<Host> StartScan(string userInput)
        {
            WriteToConsole(findingNetworkHostsMsg, ConsoleColor.Yellow);

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

                WriteToConsole($"{iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Red);

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
                i=i+numberOfThreads;

                threadList.WaitAll();

                threadList.Clear();
            }

            WriteToConsole($"Found {ActiveHosts.Count()} hosts...", ConsoleColor.Yellow);
            
            return ActiveHosts;

        }

        public void ThreadedPingRequest()
        {
            if (hostList.Any(x => x.PingAttempted is false))
            {
                var targetHost = hostList.Select(x => x).Where(x => x.PingAttempted is false).First();
                targetIp = targetHost.IP.ToString();
                hostList.Remove(targetHost);
                WriteToConsole($"Trying host: {targetIp}", ConsoleColor.Yellow);

                var result = PingHost(IPAddress.Parse(targetIp));
            }         
        }

        public string IncrementIpAddress(string ipAddress)
        {
            var ipSplit = Convert.ToString(ipAddress).Split(".");

            // Increment the last octet
            ipSplit[ipSplit.Count() - 1] = (int.Parse(ipSplit[ipSplit.Count() - 1]) + 1).ToString();

            for(int i = 3; i > 0; i--)
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

            return String.Join(".",ipSplit);
        }

        public bool PingHost(IPAddress targetIp)
        {
            var foundHosts = new List<string>();

            Host host;

            var ping = new Ping();

            int timeout = 100;
            var reply = ping.Send(targetIp, timeout);

            var result = reply.Status;

            if (result == IPStatus.Success)
            {
                foundHosts.Add(targetIp.ToString());
            }

            // Scan host to get MAC address
            host = ArpScan.Scan(targetIp.ToString());

            // All properties are null if ARP scan fails
            if (host.IP is not null)
            {
                WriteToConsole($"{host.IP}, {host.MAC}, {host.Vendor}", ConsoleColor.Green);
                ActiveHosts.Add(host);
            }

            return true;

        }

        private static void WriteToConsole(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
