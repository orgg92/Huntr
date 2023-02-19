namespace Radar.Services
{
    using Radar.Common.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Common.Util.ConsoleTools;

    public class LoggingService : ILoggingService
    {
        private string configPath = $"{System.Reflection.Assembly.GetExecutingAssembly().Location}/Logs/log.txt";

        public LoggingService()
        {

        }

        public void LogToConsole(string message, ConsoleColor color)
        {
            ConsoleTools.WriteToConsole(message, color);
        }

        public void LogToFile(string message)
        {
            // ToDo
        }
    }
}
