﻿namespace Radar.Common.Config
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ConfigConverter
    {
        public static bool ConvertConfigToBool(ConfigSetting config)
        {
            return bool.Parse(config.PropertyValue);
        }

        public static int ConvertConfigToInt(string configSetting)
        {
            return int.Parse(configSetting);
        }

        public static IEnumerable<int> ConvertToIntArray(string configSetting)
        {
            // Make compatible with comma spacing or white space.
            var settings = configSetting.Contains(" ") ? configSetting.Split(" ") : configSetting.Split(",");

            return settings.Select(x => int.Parse(x));

        }
    }
}
