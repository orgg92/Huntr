using System.Net;

namespace Radar.Services
{
    public interface IIPManipulationService
    {
        uint ReturnFirtsOctet(IPAddress ipAddress);
        string ReturnSubnetmask(IPAddress ipaddress);
    }
}