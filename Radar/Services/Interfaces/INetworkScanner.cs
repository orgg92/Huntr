using Radar.Common.NetworkModels;
using System.Net;
using System.Net.NetworkInformation;

namespace Radar.Services.Interfaces
{
    public interface INetworkScanner
    {
        string FindInterfaces();
        bool PingHost(IPAddress targetIp);
        IEnumerable<Host> StartScan(string iface);
        IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask);
    }
}