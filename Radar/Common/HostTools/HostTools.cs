using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radar.Common.HostTools
{
    public class HostTools
    {
        public Flooder Flooder { get; set; }

        public HostTools()
        {
            Flooder = new Flooder();
        }
    }
}
