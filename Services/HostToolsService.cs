namespace Radar.Services
{
    using Radar.Common.Config;
    using Radar.Common.HostTools;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Models.HostTools;
    using Radar.Services.Interfaces;
    using System.Linq.Expressions;

    public class HostToolsService : IHostToolsService
    {
        private readonly ILoggingService _loggingService;

        private PortScanner PortScanner;
        private SSHClient sshClient;

        public HostToolsService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            PortScanner = new PortScanner(_loggingService);
            sshClient = new SSHClient(_loggingService);
        }

        public void ChooseService(IEnumerable<Host> hosts)
        {
            try
            {
                var selectedHost = HostSelect(hosts);

                PortScanner.CheckHost(selectedHost.IP);

                RunPenTest:
                CommonConsole.WriteToConsole("Continue to SSH test? [Y/N]", ConsoleColor.Yellow);
                var continueToSsh = Console.ReadLine();

                if (CommonConsole.ValidateUserInput(continueToSsh)) {
                    if (continueToSsh.ToString().ToLower() == "y")
                    {
                        sshClient.AttemptConnection(selectedHost);
                    } else
                    {
                        Environment.Exit(0);
                    }
                } else
                {
                    goto RunPenTest;
                }

                        
            }
            catch (Exception e)
            {
                CommonConsole.WriteToConsole("Error loading custom config...", ConsoleColor.Red);
                throw e;
            }

        }

        private Host HostSelect(IEnumerable<Host> hosts)
        {
            CommonConsole.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            CommonConsole.WriteToConsole($"Selected: {hosts.ElementAt(selectedHost).IP}", ConsoleColor.Yellow);

            return hosts.ElementAt(selectedHost);
        }

        private int ToolSelector()
        {
            CommonConsole.WriteToConsole("Select a tool...", ConsoleColor.Yellow);

            var input = int.Parse(Console.ReadLine()) - 1;

            return input;

        }
    }
}
