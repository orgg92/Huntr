using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Radar.Common;
using System.Runtime.InteropServices;

namespace Radar.Services
{


    public class NetworkScanner : INetworkScanner
    {

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP,
                                 byte[] pMacAddr, ref uint PhyAddrLen);

        private const string spacer = "***********************************************************";

        private string foundInterfacesMsg = "Found {0} interfaces...",
                       findingInterfacesMsg = "Searching for network interfaces...",
                       findingNetworkHostsMsg = "Searching for hosts...",
                       foundNetworkHostsMsg = "Found {0} hosts...",
                       firstHost = String.Empty,
                       lastHost = String.Empty;


        private SubnetsList _subnetList;
        private string hostIP = String.Empty;
        private int interfaceCount;

        private NetworkInterface[] ifaces;
        private UnicastIPAddressInformationCollection ipAddresses;
        private IPAddress ipAddress;
        private string subnetMask;

        private List<Host> ActiveHosts = new List<Host>();

        private string input = String.Empty;

        private byte[] macAddr = new byte[6];
        private uint macAddrLen;

        private readonly IIPManipulationService _iPManipulationService;

        public NetworkScanner(IIPManipulationService iPManipulationService)
        {
            _subnetList = new SubnetsList();
            _iPManipulationService = iPManipulationService;

            ifaces = NetworkInterface.GetAllNetworkInterfaces();
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

                return input;

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool ScanInterfaces(string userInput)
        {
            WriteToConsole(findingNetworkHostsMsg, ConsoleColor.Yellow);
            var iface = ifaces[int.Parse(userInput)-1];
            try
            {

                ipAddresses = iface.GetIPProperties().UnicastAddresses;
                var subnetMaskTest = ipAddresses.Where(x => x.IPv4Mask.ToString() != "0.0.0.0").Select(x => x.IPv4Mask).First();

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First();

                WriteToConsole($"{iface.Name}: {ipAddress} / {subnetMaskTest} ", ConsoleColor.Red);

                ScanNetwork(ipAddress, subnetMaskTest.ToString());
            } catch (Exception e)
            {

            }

            return true;
        }

        public void ScanNetwork(IPAddress ipAddress, string subnetMask) // host IP and subnet
        {
            // white list the IP of this host so we don't scan it
            hostIP = ipAddress.ToString();

            var foundHosts = new List<string>();

            // Calculate number of hosts from subnet mask

            
            var subnet = _subnetList.ReturnSubnetInfo(subnetMask);

            var segment = new IPSegment(ipAddress.ToString(), subnet.SubnetMask);
            Console.WriteLine(segment.NetworkAddress.ToString(), segment.BroadcastAddress);

            // Calculate first IP to scan based on input IP
            lastHost = segment.Hosts().Last().ToIpString();
            firstHost = segment.Hosts().First().ToIpString();
            var targetIp = firstHost;

            //// Loop through and scan

            for (int i = 0; i < subnet.NumberOfHosts; i++)
            {

                WriteToConsole($"Trying host: {targetIp}", ConsoleColor.Yellow);

                var result = PingHost(IPAddress.Parse(targetIp));

                if (result == IPStatus.Success)
                {
                    foundHosts.Add(targetIp);
                    Console.WriteLine($"Found new host: {targetIp}");

                    ArpScan.Scan(targetIp);
                }
                else
                {
                    // Use other means to determine if host is alive - aka arping

                    ArpScan.Scan(targetIp);
                }



                targetIp = IncrementIpAddress(targetIp);
            }

            Console.WriteLine($"Found {foundHosts.Count()} hosts...");

            foreach(var host in foundHosts)
            {
                Console.WriteLine(host);
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
                        Console.WriteLine((int.Parse(ipSplit[i - 1]) + 1).ToString());
                        ipSplit[i - 1] = (int.Parse(ipSplit[i - 1]) + 1).ToString();
                        ipSplit[i] = "0";
                    }
                }
            }

            return String.Join(".",ipSplit);
        }

        public IPStatus PingHost(IPAddress targetIp)
        {
            // Scan network on interface
            var ping = new Ping();

            int timeout = 100;
            var reply = ping.Send(targetIp, timeout);

            return reply.Status;
        }

        private static void WriteToConsole(string msg, ConsoleColor color)
        {
            Console.WriteLine(msg, ConsoleColor.Blue);
        }
    }
}
