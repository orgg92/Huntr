namespace Radar.Common.Config
{
    using Radar.Common.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Net.Mime.MediaTypeNames;

    public class Config
    {
        public static string ConfigPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Config/config.ini";
        public static string BaseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool CUSTOM_PORT_SCAN { get; set; }
        public static bool CUSTOM_IP_SCAN { get; set; }
        public static bool CUSTOM_MAC_SCAN { get; set; }
        public static string LOG_FILE_PATH { get; set; }
        public static string MAC_LIST_PATH { get; set; }
        public static string PORT_LIST_PATH { get; set; }
        public static IEnumerable<int> CUSTOM_PORTS { get; set; }

        public static void BuildConfig()
        {
            var configSettings = ConfigLoader.LoadConfig();
            var CPS = RetrieveValue(configSettings, "CUSTOM_PORT_SCAN");
            var CIS = RetrieveValue(configSettings, "CUSTOM_IP_SCAN");
            var LFP = RetrieveValue(configSettings, "LOG_FILE_PATH");
            var MLP = RetrieveValue(configSettings, "MAC_LIST_PATH");
            var PLP = RetrieveValue(configSettings, "PORT_LIST_PATH");
            var CP = RetrieveValue(configSettings, "CUSTOM_PORTS");

            CUSTOM_PORT_SCAN = ConfigConverter.ConvertConfigToBool(CPS);
            CUSTOM_IP_SCAN = ConfigConverter.ConvertConfigToBool(CIS);
            LOG_FILE_PATH = LFP.PropertyValue;
            MAC_LIST_PATH = MLP.PropertyValue;
            PORT_LIST_PATH = PLP.PropertyValue;
            CUSTOM_PORTS = ConfigConverter.ConvertToIntArray(CP.PropertyValue);


        }

        public static ConfigSetting RetrieveValue(List<ConfigSetting> config, string propertyName)
        {
            return config.Where(y => y.PropertyName == propertyName).First();
        }
    }
}
