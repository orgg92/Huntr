using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Radar.Services.Interfaces;

namespace Radar.Services
{
    public class IPManipulationService : IIPManipulationService
    {
        public IPManipulationService()
        {

        }

        public string ReturnSubnetmask(IPAddress ipaddress)
        {
            uint firstOctet = ReturnFirtsOctet(ipaddress);
            if (firstOctet >= 0 && firstOctet <= 127)
                return "255.0.0.0";
            else if (firstOctet >= 128 && firstOctet <= 191)
                return "255.255.0.0";
            else if (firstOctet >= 192 && firstOctet <= 223)
                return "255.255.255.0";
            else return "0.0.0.0";
        }

        public uint ReturnFirtsOctet(IPAddress ipAddress)
        {
            byte[] byteIP = ipAddress.GetAddressBytes();
            uint ipInUint = (uint)byteIP[0];
            return ipInUint;
        }
    }
}
