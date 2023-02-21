namespace Radar.Common.Config
{
    using System.Reflection;

    public static class Config
    {
        public const bool EnableTools = false;
        public const bool EnableHostTools = true;

        public static readonly int[] Ports = { 20, 22, 7027, 5027, 80, 53, 443 };

        public static string ConfigPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Config/config.ini";
        public static string BaseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
