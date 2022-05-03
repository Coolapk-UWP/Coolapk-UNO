using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CoolapkUNO.Helpers
{
    public enum MessageType
    {
        Message,
        NoMore,
        NoMoreReply,
        NoMoreLikeUser,
        NoMoreShare,
        NoMoreHotReply,
    }

    public static class AppUtils
    {
        public static event EventHandler<(MessageType Type, string Message)> NeedShowInAppMessageEvent;

        internal static void ShowInAppMessage(MessageType type, string message = null)
        {
            NeedShowInAppMessageEvent?.Invoke(null, (type, message));
        }

        public static void ShowHttpExceptionMessage(HttpRequestException e)
        {
            if (e.Message.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) != -1)
            { NeedShowInAppMessageEvent?.Invoke(null, (MessageType.Message, $"服务器错误： {e.Message.Replace("Response status code does not indicate success: ", string.Empty)}")); }
            else if (e.Message == "An error occurred while sending the request.") { NeedShowInAppMessageEvent?.Invoke(null, (MessageType.Message, "无法连接网络。")); }
            else { NeedShowInAppMessageEvent?.Invoke(null, (MessageType.Message, $"请检查网络连接。 {e.Message}")); }
        }

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

        public static string GetBase64(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }
    }
}
