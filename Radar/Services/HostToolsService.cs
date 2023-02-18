using Radar.Common.HostTools;
using Radar.Common.NetworkModels;
using Radar.Common.Util;
using Radar.Services.Interfaces;
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
        private const string IP = "IP",
                             MAC = "MAC",
                             Vendor = "Vendor",
                             No = "#";

        private const int IP_Length = 15,
                          Vendor_Length = 64;


        private Flooder Flooder { get; set; }
        private const string TableHeader = "  # |       IP        |         MAC       |                      Vendor                       |";
        private string TableHead = String.Format("{0,3} | {1,10} | {2,5} | {3,5} |", No, IP, MAC, Vendor);

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
            }
            catch (Exception e)
            {

            }
        }

        private string PadIP(string IP)
        {
            var count = IP.Length;
            var IPMaxLength = 15;

            var sb = new StringBuilder();
            sb.Append("  ");
            sb.Append(IP);
            sb.Append(' ', IPMaxLength - count);

            
            return sb.ToString();
        }

        //private string PadVariable(dynamic variable)
        //{

        //}

        private string PadVendor(string vendor)
        {
            var count = 50;
            var sb = new StringBuilder();

            sb.Append(" ");
            sb.Append(vendor);
            sb.Append(' ', count - vendor.Length);
            return sb.ToString();
        }

        private Host HostPrompt(IEnumerable<Host> hosts)
        {

            ConsoleTools.WriteToConsole(TableHeader, ConsoleColor.Red);

            for (int i = 0; i < hosts.Count(); i++)
            {
                ConsoleTools.WriteToConsole(String.Format("{0,3} |{1}| {2,5} |{3}|", i+1, PadIP(hosts.ToList()[i].IP), hosts.ToList()[i].MAC, PadVendor(hosts.ToList()[i].Vendor)), ConsoleColor.Red);
                    //)({i + 1}) {hosts.ToList()[i].IP} | {hosts.ToList()[i].MAC} | {hosts.ToList()[i].Vendor}  ", ConsoleColor.Red);
            }

            ConsoleTools.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            return hosts.ToList()[selectedHost];
        }


    }
}
