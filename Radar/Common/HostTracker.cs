using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radar.Common
{
    public class HostTracker
    {
        public IEnumerable<AbstractHost> hosts;
    }

    public class AbstractHost
    {
        public string IP { get; set; }
        public bool PingAttempted { get; set; }
        public bool Alive { get; set; }

        public AbstractHost()
        {
            PingAttempted = false;
            Alive = false;
        }
    }


}
