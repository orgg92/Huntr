namespace Radar.Services
{
    using Radar.Common.NetworkModels;
    using System;

    public interface ILoggingService
    {
        void LogToConsole(string message, ConsoleColor color);
        void LogToFile(string[] textArray);
        string[] DisplayHostList(IEnumerable<Host> hosts);
    }
}