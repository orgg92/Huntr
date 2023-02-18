using Radar.Common.NetworkModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Radar.Common
{

    // Used to display the results of the network scan, a little bit inefficient but the trade off for convenience makes it worth while

    public class StringTableFormatter
    {
        public static string PadIP(string IP)
        {
            var count = IP.Length;
            var IPMaxLength = 15;

            var sb = new StringBuilder();
            sb.Append("  ");
            sb.Append(IP);
            sb.Append(' ', IPMaxLength - count);


            return sb.ToString();
        }

        public static string PadVendor(string vendor)
        {
            var count = 50;
            var sb = new StringBuilder();

            sb.Append(" ");
            sb.Append(vendor);
            sb.Append(' ', count - vendor.Length);
            return sb.ToString();
        }

        public static string PadHostname(string hostname, string tableHeaderMsg)
        {
            var count = tableHeaderMsg.Length;
            var sb = new StringBuilder();

            sb.Append(" ");
            sb.Append(hostname);
            sb.Append(' ', (count - 3) - hostname.Length);

            return sb.ToString();
        }

        public static Host PadPropertiesForDisplay(Host host, string tableHeaderMsg)
        {
            var newHost = new Host()
            {
                IP = PadIP(host.IP),
                Vendor = PadVendor(host.Vendor),
                HostName = PadHostname(host.HostName, tableHeaderMsg),
                MAC = host.MAC
            };            
                    
            return newHost;
        }
    }
}
