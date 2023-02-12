using Radar.Common;
using System.Net;
using System.Net.NetworkInformation;

namespace Radar.Services
{
    public interface INetworkScanner
    {
        string FindInterfaces();
        IPStatus PingHost(IPAddress targetIp);
        IEnumerable<Host> ScanInterfaces(string iface);
        IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask);
    }
}