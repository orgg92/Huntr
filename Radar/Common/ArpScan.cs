// Borrowed this from giuliocomi @ github: https://github.com/giuliocomi/arp-scanner

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Radar.Common
{
    public class ArpScan
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        private static uint macAddrLen = (uint)new byte[6].Length;
        private const string separator = "|";
        private static List<string> macList = new List<string>();

        public ArpScan()
        {


        }

        public static Host Scan(string ipAddress)
        {
            int timeout = 4000;
            Host host = new Host();
            macList = LoadListFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/MacList.txt");

            //FormatOutput("Starting ARP scan", ConsoleColor.Cyan);
            host = CheckStatus(ipAddress, timeout);
            //Console.WriteLine(String.Format("{0,-20} | {1,-20} | {2,-20}", "IP", "MAC", "InterfaceDetails"));
            if (host.IP is not null)
            {
                //
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(String.Format("{0,-20} | {1,-20} | {2,-20}", host.IP, host.MAC, host.Vendor));
                Console.ResetColor();
            }

            return host;

        }

        private static string MacAddresstoString(byte[] macAdrr)
        {
            string macString = BitConverter.ToString(macAdrr);
            return macString.ToUpper();
        }

        private static void ThreadedARPRequest(string ipString, ref Host result)
        {
            byte[] macAddr = new byte[6];

            var ipAddress = IPAddress.Parse(ipString);
            SendARP((int)BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen);
            if (MacAddresstoString(macAddr) != "00-00-00-00-00-00")
            {
                string macString = MacAddresstoString(macAddr);
                result = new Host() 
                {
                    IP = ipString, 
                    MAC = macString,
                    Vendor = GetDeviceInfoFromMac(macString)
                };
            }
        }

        private static string GetDeviceInfoFromMac(string mac)
        {
            string pattern = mac.Substring(0, 8) + ".*";

            try
            {
                foreach (var entry in macList)
                {
                    Match found = Regex.Match(entry, pattern);
                    if (found.Success)
                    {
                        return found.Value.Split(separator[0])[1];
                    }
                }
            }
            catch (Exception e)
            {
                FormatOutput(e.ToString(), ConsoleColor.Red); 
            }
            return "Unknown";
        }

        public static Host CheckStatus(string ipAddress, int timeout)
        {
            Host result = new Host();
            byte[] macAddr = new byte[6];

            try
            {
                    Thread threadARP = new Thread(() => ThreadedARPRequest(ipAddress, ref result));
                    threadARP.Start();
            }
            catch (Exception e)
            {
                FormatOutput(e.ToString(), ConsoleColor.Red); 
            }

            Thread.Sleep(timeout);
            return result;
        }

        private static List<string> LoadListFromFile(string filename)
        {
            List<string> list = new List<string>();

            try
            {
                foreach (var ipAddress in File.ReadAllLines(filename))
                    list.Add(ipAddress.Trim());
            }
            catch (Exception e)
            {
                FormatOutput("Error reading file.", ConsoleColor.Red);

                return new List<string>();
            }
            return list;
        }


        private static void FormatOutput(string message, System.ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}