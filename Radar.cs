namespace Radar
{
    using Radar.Common.Config;
    using Radar.Common.NetworkModels;
    using Radar.Common.Util;
    using Radar.Services;
    using Radar.Services.Interfaces;

    public class RadarScanner
    {
        private readonly INetworkScanner _networkScanner;
        private readonly IHostToolsService _hostToolsService;
        private readonly ILoggingService _loggingService;

        private IEnumerable<Host> Hosts;

        public const string FeatureSelection = "Select one of the following options...";

        public RadarScanner(INetworkScanner networkScanner, IHostToolsService hostToolsService, ILoggingService loggingService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            _loggingService = loggingService;
        }

        public void StartApp()
        {
            Config.BuildConfig();
            var loggingText = RunPhase1();
            LoggingPrompt(loggingText);
            RunPhase2();

        }

        public string[] RunPhase1()
        {
            Hosts = Enumerable.Empty<Host>();

        Input:
            var ifaces = _networkScanner.FindInterfaces();

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
            CommonConsole.WriteToConsole("Select an interface to scan on...", ConsoleColor.Yellow);

            var input = Console.ReadLine();

            // not ideal but prevents any other selections
            if (!ValidateInput(input, ifaces.Length))
            {
                InvalidSelection();
                goto Input;
            }
            else
            {
                Hosts = _networkScanner.StartScan(input);
            }

            var formattedText = _loggingService.DisplayHostList(Hosts);

            return formattedText;

        }

        private void LoggingPrompt(string[] textArray)
        {
        LoggingPrompt:
            CommonConsole.WriteToConsole("Write to logfile? [Y/N]", ConsoleColor.Yellow);
            var logging = Console.ReadLine();

            if (logging.ToLower() == "y")
            {
                CommonConsole.WriteToConsole("Writing to logfile...", ConsoleColor.Yellow);
                _loggingService.LogToFile(textArray);
            }
            else if (logging.ToLower() == "n")
            {
                CommonConsole.WriteToConsole("Skipping log file", ConsoleColor.Yellow);
            }
            else
            {
                CommonConsole.WriteToConsole(CommonConsole.InvalidSelection, ConsoleColor.Red);
                goto LoggingPrompt;
            }

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
        }

        public void InvalidSelection()
        {
            CommonConsole.WriteToConsole("Invalid selection", ConsoleColor.Red);
        }

        public bool ValidateInput(string input, int interfaceCount)
        {
            if (int.TryParse(input, out var parsedInput))
            {
                if (parsedInput < interfaceCount)
                    return true;

                return false;
            }
            else
            {
                return false;
            }
        }

        private void RunPhase2()
        {
            _hostToolsService.ChooseService(Hosts);
        }

        //public List<ConfigSetting> BuildConfig()
        //{
        //    CommonConsole.WriteToConsole("Loading config...", ConsoleColor.Yellow);
        //    Config.BuildConfig();
        //}

    }
}

