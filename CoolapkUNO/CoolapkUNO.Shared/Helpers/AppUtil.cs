using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CoolapkUNO.Helpers
{
    public static class AppUtil
    {
        private static readonly DateTime UnixDateBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int DateTimeToTimeStamp(DateTime date)
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            int seconds = Convert.ToInt32(ts.TotalSeconds);
            return seconds;
        }
        
        public static double DateTimeToUnixTimeStamp(DateTime date)
        {
            return Math.Round(date.ToUniversalTime()
                    .Subtract(UnixDateBase)
                    .TotalSeconds);
        }

        public static string GetMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var r1 = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var r2 = BitConverter.ToString(r1).ToLowerInvariant();
                return r2.Replace("-", "");
            }
        }
    }
}
