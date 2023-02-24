﻿namespace Radar.Common.Config
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

        public static string Height = Console.WindowHeight.ToString();
        public static string Width = Console.WindowWidth.ToString();

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

        public static List<ConfigSetting> ConfigSettings { get; set; }

        public static void BuildConfig()
        {
            // Windows only
            Console.SetWindowSize(120, 30);

            ConfigSettings = new List<ConfigSetting>();

            CommonConsole.WriteToConsole("Loading config...", ConsoleColor.Yellow);

            LoadConfig();
            var CPS = RetrieveValue(ConfigSettings, "CUSTOM_PORT_SCAN");
            var CIS = RetrieveValue(ConfigSettings, "CUSTOM_IP_SCAN");
            var LFP = RetrieveValue(ConfigSettings, "LOG_FILE_PATH");
            var MLP = RetrieveValue(ConfigSettings, "MAC_LIST_PATH");
            var PLP = RetrieveValue(ConfigSettings, "PORT_LIST_PATH");
            var CP  = RetrieveValue(ConfigSettings, "CUSTOM_PORTS");
            var CIA = RetrieveValue(ConfigSettings, "CUSTOM_IP_ADDRESSES").PropertyValue;

            CUSTOM_PORT_SCAN = ConfigConverter.ConvertConfigToBool(CPS);
            CUSTOM_IP_SCAN = ConfigConverter.ConvertConfigToBool(CIS);
            LOG_FILE_PATH = LFP.PropertyValue;
            MAC_LIST_PATH = !String.IsNullOrEmpty(MLP.PropertyValue) ? MLP.PropertyValue : $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Resources/MacList.txt";
            PORT_LIST_PATH = !String.IsNullOrEmpty(PLP.PropertyValue) ? PLP.PropertyValue : $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Common/Resources/PortList.txt";
            CUSTOM_PORTS = ConfigConverter.ConvertToIntArray(CP.PropertyValue);
            CUSTOM_IP_ADDRESSES = ConfigConverter.ConvertToStringArray(CIA);

            LoadMACList();

            ConfigSettings.Clear();
        }

        public static void LoadMACList()
        {
            MAC_LIST = CommonOperations.LoadListFromFile(Config.MAC_LIST_PATH);
        }

        public static ConfigSetting RetrieveValue(List<ConfigSetting> config, string propertyName)
        {
            return config.Where(y => y.PropertyName == propertyName).First();
        }

        public static bool LoadConfig()
        {
            try
            {
                CreateFileIfNotExists();
                CommonOperations.LoadListFromFile(Config.ConfigPath).ForEach(x => ConfigSettings.Add(BuildSetting(x)));
                return true;
            }
            catch
            {
                CommonConsole.WriteToConsole("Error loading config file...", ConsoleColor.Red);
                Environment.Exit(-1);
                return false;
            }
        }

        public static ConfigSetting BuildSetting(string configString)
        {
            var configPair = configString.Split('=');

            var configSetting = new ConfigSetting { PropertyName = configPair[0], PropertyValue = configPair[1] };
            ConfigSettingValidator validator = new ConfigSettingValidator();

            var result = validator.Validate(configSetting);

            if (!result.IsValid)
            {
                throw new Exception();
            }

            return configSetting;
        }

        public static void CreateFileIfNotExists()
        {
            var commonDir = $"{Config.BaseDirectory}/Common/";
            var configDir = $"{commonDir}/Config/";

            if (!System.IO.Directory.Exists(commonDir))
            {
                System.IO.Directory.CreateDirectory(commonDir);

                if (!System.IO.Directory.Exists(configDir))
                    System.IO.Directory.CreateDirectory(configDir);
            }

            if (!File.Exists(Config.ConfigPath))
            {
                File.Create(Config.ConfigPath);
                CommonOperations.WriteToFile(Config.ConfigPath, CreateConfigTemplate());
            }

        }

        public static string CreateConfigTemplate()
        {
            return  "CUSTOM_PORT_SCAN=false\r\n" +
                    "CUSTOM_IP_SCAN=false\r\n" +
                    "LOG_FILE_PATH='C:/Test/Dir/File.txt'\r\n" +
                    "MAC_LIST_PATH='C:/Test/Dir/File.txt'\r\n" +
                    "PORT_LIST_PATH='C:/Test/Dir/File.txt'\r\n" +
                    "CUSTOM_PORTS='1 2 80 8008'" +
                    "CUSTOM_IP_ADDRESSES=''";
        }
    }
}
