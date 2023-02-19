namespace Radar.Common
{
    public static class CommonConsole
    {
        // misc variables used for displaying to console

        public const string spacer = "***********************************************************";
        public const string separator = "|";
        public const string InvalidSelection = "Invalid Selection";

        public static string TableHeader = String.Empty;

        // Used to display results
        public static readonly string[] TableHeaderMessages = {
            "  # ",
            "       IP        ",
            "         MAC       ",
            "                      Vendor                       ",
            "         Hostname       "
        };
    }
}
