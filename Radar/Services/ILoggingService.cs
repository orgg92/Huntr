namespace Radar.Services
{
    using System;

    public interface ILoggingService
    {
        void LogToConsole(string message, ConsoleColor color);
        void LogToFile(string message);
    }
}