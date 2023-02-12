using Radar.Common;
using Radar.Common.HostTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Radar.Services
{
    public class HostToolsService : IHostToolsService
    {
        private Flooder Flooder { get; set; }

        public HostToolsService()
        {
            Flooder = new Flooder();
        }

        public void ChooseService(IEnumerable<Host> hosts)
        {
            try
            {
                var targetHost = new Host();

                if (hosts.Count() == 0)
                {
                    ConsoleTools.WriteToConsole("No hosts detected... Enter target IP", ConsoleColor.Red);
                    var targetIP = Console.ReadLine();

                    targetHost.IP = targetIP;

                }
                else
                {
                    targetHost = HostPrompt(hosts);
                }

                ConsoleTools.WriteToConsole("Select a tool...", ConsoleColor.Yellow);
                var input = int.Parse(Console.ReadLine()) - 1;

                switch (input)
                {
                    case 0:
                        this.Flooder.GenerateTraffic(targetHost);
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }

        private Host HostPrompt(IEnumerable<Host> hosts)
        {
            for (int i = 0; i < hosts.Count(); i++)
            {
                ConsoleTools.WriteToConsole($"({i + 1}) {hosts.ToList()[i].IP} / {hosts.ToList()[i].MAC} / {hosts.ToList()[i].Vendor}  ", ConsoleColor.Red);
            }

            ConsoleTools.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            return hosts.ToList()[selectedHost];
        }


    }
}
