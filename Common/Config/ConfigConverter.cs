namespace Radar.Common.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ConfigConverter
    {
        public static bool ConvertConfigToBool(List<ConfigSetting> config)
        {
            return bool.Parse(config.Where(y => y.PropertyName == "FULL_PORT_SCAN").Select(x => x.PropertyValue).First());
        }

        //public static int ConvertConfigToInt(string configSetting)
        //{

        //}

        public static IEnumerable<int> ConvertToIntArray(string configSetting)
        {
            // Make compatible with comma spacing or white space.
                var settings = configSetting.Contains(" ") ? configSetting.Split(" ") : configSetting.Split(",");

                return settings.Select(x => int.Parse(x));

        }
    }
}
