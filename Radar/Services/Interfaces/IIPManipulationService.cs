using System.Net;

namespace Radar.Services.Interfaces
{
    public interface IIPManipulationService
    {
        uint ReturnFirtsOctet(IPAddress ipAddress);
        string ReturnSubnetmask(IPAddress ipaddress);
    }
}