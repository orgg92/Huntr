namespace Radar.Common.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileReader
    {

        public static List<string> LoadListFromFile(string filename)
        {
            List<string> list = new List<string>();

            try
            {
                foreach (var line in File.ReadAllLines(filename))
                    list.Add(line.Trim());
            }
            catch (Exception e)
            {
                ConsoleTools.WriteToConsole("Error reading file.", ConsoleColor.Red);

                return new List<string>();
            }
            return list;
        }
    }
}
