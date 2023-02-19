namespace Radar.Common
{
    using Radar.Common.NetworkModels;
    using System.Text;

    // Used to display the results of the network scan, a little bit inefficient but the trade off for convenience makes it worth while

    public class StringTableFormatter
    {
        public static string PadIP(string IP, StringBuilder sb)
        {
            sb.Clear();

            var count = IP.Length;
            var IPMaxLength = 15;

            sb.Append("  ");
            sb.Append(IP);
            sb.Append(' ', IPMaxLength - count);

            return sb.ToString();
        }

        public static string PadVendor(string vendor, StringBuilder sb)
        {
            sb.Clear();

            var count = 50;

            sb.Append(" ");
            sb.Append(vendor);
            sb.Append(' ', count - vendor.Length);

            return sb.ToString();
        }

        public static string PadHostname(string hostname, string tableHeaderMsg, StringBuilder sb)
        {
            sb.Clear();

            var count = tableHeaderMsg.Length;

            sb.Append(" ");
            sb.Append(hostname);
            sb.Append(' ', (count - 3) - hostname.Length);

            return sb.ToString();
        }

        public static Host PadPropertiesForDisplay(Host host, string tableHeaderMsg)
        {
            var sb = new StringBuilder();

            var newHost = new Host()
            {
                IP = PadIP(host.IP, sb),
                Vendor = PadVendor(host.Vendor, sb),
                HostName = PadHostname(host.HostName, tableHeaderMsg, sb),
                MAC = host.MAC
            };

            return newHost;
        }
    }
}
