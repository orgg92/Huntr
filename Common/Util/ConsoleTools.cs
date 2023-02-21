namespace Radar.Common.Util
{
    public static class ConsoleTools
    {
        public static void WriteToConsole(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
