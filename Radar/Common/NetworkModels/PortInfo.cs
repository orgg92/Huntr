namespace Radar.Common.NetworkModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PortInfo
    {
        public int PortNum { get; set; }
        public string PortName { get; set; }
        public bool Attempted { get; set; }

        public PortInfo()
        {

        }

        public PortInfo(string portName, string portNum)
        {
            this.PortName = portName;
            this.PortNum = int.Parse(portNum);

        }
    }


}
