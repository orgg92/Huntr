using Radar.Common;
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

        // Used to display results later
        private string[] TableHeaderMessages = new string[] { 
            "  # ",
            "       IP        ",
            "         MAC       ",
            "                      Vendor                       ",
            "         Hostname       "
        };


        private Flooder Flooder { get; set; }
        private string TableHeader = String.Empty;

        public HostToolsService()
        {
            TableHeader = $"{TableHeaderMessages[0]}|{TableHeaderMessages[1]}|{TableHeaderMessages[2]}|{TableHeaderMessages[3]}|{TableHeaderMessages[4]}|";
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

        private Host HostPrompt(IEnumerable<Host> hosts)
        {

            ConsoleTools.WriteToConsole(TableHeader, ConsoleColor.Red);

            for (int i = 0; i < hosts.Count(); i++)
            {
                // create host for display to console
                var host = hosts.Select(x => new Host() { HostName = x.HostName, MAC = x.MAC, IP = x.IP, Vendor = x.Vendor }).ElementAt(i);
                var paddedHost = StringTableFormatter.PadPropertiesForDisplay(host, TableHeaderMessages[4]);

                ConsoleTools.WriteToConsole(
                    String.Format(
                        "{0,3} |{1}| {2,5} |{3}| {4} |",
                        i + 1,
                        paddedHost.IP,
                        paddedHost.MAC,
                        paddedHost.Vendor,
                        paddedHost.HostName),
                ConsoleColor.Red);

            }

            ConsoleTools.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            return hosts.ElementAt(selectedHost);
        }
    }
}
