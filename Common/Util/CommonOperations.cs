namespace Radar.Common.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class CommonOperations
    {
        public static byte[] ConvertStringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static void WriteToFile(string fileName, string value)
        {
            using (FileStream fs = File.OpenWrite(fileName))
            {
                byte[] buffer;

                buffer = CommonOperations.ConvertStringToBytes(value);
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
