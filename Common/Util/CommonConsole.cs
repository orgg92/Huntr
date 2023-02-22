namespace Radar.Common.Util
{
    public static class CommonConsole
    {
        // misc variables used for displaying to console

        public const string spacer = "***********************************************************";
        public const string separator = "|";
        public const string InvalidSelection = "Invalid Selection";

        public static string TableHeader = string.Empty;

        public static readonly string[] PortTableHeaderMessages = { "  Port     ", "   Service                                 " };

        // Used to display results - not pretty but looks okay in the console
        public static readonly string[] DeviceTableHeaderMessages = {
            "  # ",
            "       IP        ",
            "         MAC       ",
            "                      Vendor                       ",
            "         Hostname       "
        };
    }
}
