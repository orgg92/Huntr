namespace Radar.Common.Util
{
    using Radar.Common.Config;
    using System.Collections.Generic;

    public static class ConfigLoader
    {
        public static List<ConfigSetting> LoadConfig()
        {
            var configProfile = new List<ConfigSetting>();
            CreateFileIfNotExists();
            FileReader.LoadListFromFile(Config.ConfigPath).ForEach(x => configProfile.Add(BuildSetting(x)));
            return configProfile;
        }

        public static ConfigSetting BuildSetting(string configString)
        {
            var configPair = configString.Split('=');
            return new ConfigSetting { PropertyName = configPair[0], PropertyValue = configPair[1] };
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
            return "LOG_FILE_PATH='C:/Test/Dir/File.txt'\r\nMAC_LIST_PATH='C:/Test/Dir/File.txt'\r\nPORT_LIST_PATH='C:/Test/Dir/File.txt'\r\nFULL_PORT_SCAN='true'\r\nCUSTOM_PORTS='1 2 80 8008'";
        }
    }
}
