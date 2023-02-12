using System.Net;
using System.Net.NetworkInformation;

namespace Radar.Services
{
    public interface INetworkScanner
    {
        string FindInterfaces();
        IPStatus PingHost(IPAddress targetIp);
        bool ScanInterfaces(string iface);
        void ScanNetwork(IPAddress ipAddress, string subnetMask);
    }
}