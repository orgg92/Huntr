// Some code borrowed and refactored from giuliocomi @ github: https://github.com/giuliocomi/arp-scanner

using ArpLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
            int timeout = 2000;
            Host host = new Host();
            macList = LoadListFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/MacList.txt");
            host = CheckStatus(ipAddress, timeout);

            return host;

        }

        private static string MacAddresstoString(byte[] macAdrr)
        {
            string macString = BitConverter.ToString(macAdrr);
            return macString.ToUpper();
        }

        private static Host LookupMAC(string ipString, Host result)
        {
            byte[] macAddr = new byte[6];

            var ipAddress = IPAddress.Parse(ipString);

            PhysicalAddress mac = Arp.Lookup(IPAddress.Parse(ipString));

            if (mac is not null)
            {
                var paddedMAC = PadMACString(mac?.ToString());

                result = new Host()
                {
                    IP = ipString,
                    MAC = paddedMAC,
                    Vendor = GetDeviceInfoFromMac(paddedMAC)
                };
            }

            return result;
        }

        private static string PadMACString(string mac)
        {
            var formattedMAC = Regex.Replace(mac, ".{2}", "$0-");
            return formattedMAC.Substring(0, formattedMAC.Length - 1);
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
                ConsoleTools.WriteToConsole(e.ToString(), ConsoleColor.Red); 
            }
            return "Unknown";
        }

        public static Host CheckStatus(string ipAddress, int timeout)
        {
            Host result = new Host();
            byte[] macAddr = new byte[6];

            try
            {
                result = LookupMAC(ipAddress, result);
            }
            catch (Exception e)
            {
                ConsoleTools.WriteToConsole(e.ToString(), ConsoleColor.Red); 
            }

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
                ConsoleTools.WriteToConsole("Error reading file.", ConsoleColor.Red);

                return new List<string>();
            }
            return list;
        }

    }
}