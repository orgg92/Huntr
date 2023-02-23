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
        public static string LOG_FILE_PATH { get; set; }
        public static string MAC_LIST_PATH { get; set; }
        public static string PORT_LIST_PATH { get; set; }
        public static IEnumerable<int> CUSTOM_PORTS { get; set; }
        public static IEnumerable<string> MAC_LIST { get; set; }
        public static IEnumerable<string> CUSTOM_IP_ADDRESSES { get; set; }

        public static void BuildConfig()
        {
            var configSettings = ConfigLoader.LoadConfig();
            var CPS = RetrieveValue(configSettings, "CUSTOM_PORT_SCAN");
            var CIS = RetrieveValue(configSettings, "CUSTOM_IP_SCAN");
            var LFP = RetrieveValue(configSettings, "LOG_FILE_PATH");
            var MLP = RetrieveValue(configSettings, "MAC_LIST_PATH");
            var PLP = RetrieveValue(configSettings, "PORT_LIST_PATH");
            var CP = RetrieveValue(configSettings, "CUSTOM_PORTS");
            var CIA = RetrieveValue(configSettings, "CUSTOM_IP_ADDRESSES").PropertyValue;

            CUSTOM_PORT_SCAN = ConfigConverter.ConvertConfigToBool(CPS);
            CUSTOM_IP_SCAN = ConfigConverter.ConvertConfigToBool(CIS);
            LOG_FILE_PATH = LFP.PropertyValue;
            MAC_LIST_PATH = !String.IsNullOrEmpty(MLP.PropertyValue) ? MLP.PropertyValue : $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Resources/MacList.txt";
            PORT_LIST_PATH = !String.IsNullOrEmpty(PLP.PropertyValue) ? PLP.PropertyValue : $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Resources/PortList.txt";
            CUSTOM_PORTS = ConfigConverter.ConvertToIntArray(CP.PropertyValue);
            CUSTOM_IP_ADDRESSES = ConfigConverter.ConvertToStringArray(CIA);

           LoadMACList();
        }

        public static void LoadMACList()
        {
            MAC_LIST = CommonOperations.LoadListFromFile(Config.MAC_LIST_PATH);
        }

        public static ConfigSetting RetrieveValue(List<ConfigSetting> config, string propertyName)
        {
            return config.Where(y => y.PropertyName == propertyName).First();
        }
    }
}
