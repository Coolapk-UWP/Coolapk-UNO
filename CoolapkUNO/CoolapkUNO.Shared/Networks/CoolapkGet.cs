using CoolapkUNO.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CoolapkUNO.Networks
{
    public static class CoolapkGet
    {
        private static bool IsInternetAvailable => Microsoft.Toolkit.Uwp.Connectivity.NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable;
        private static readonly Dictionary<Uri, Dictionary<int, (DateTime date, string data)>> ResponseCache = new Dictionary<Uri, Dictionary<int, (DateTime, string)>>();
        private static readonly object locker = new object();

        internal static readonly Timer CleanCacheTimer = new Timer(o =>
        {
            if (IsInternetAvailable)
            {
                DateTime now = DateTime.Now;
                lock (locker)
                {
                    foreach (KeyValuePair<Uri, Dictionary<int, (DateTime date, string data)>> i in ResponseCache)
                    {
                        int[] needDelete = (from j in i.Value
                                            where (now - j.Value.date).TotalMinutes > 2
                                            select j.Key).ToArray();
                        foreach (int item in needDelete)
                        {
                            _ = ResponseCache[i.Key].Remove(item);
                        }
                    }
                }
            }
        }, null, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));

        public static async Task<(bool isSucceed, JToken result)> GetDataAsync(Uri uri, bool isBackground, bool forceRefresh = false)
        {
            string json = string.Empty;
            (int page, Uri uri) info = uri.GetPage();
            (bool isSucceed, JToken result) result;

            void ReadCache()
            {
                lock (locker)
                {
                    json = ResponseCache[info.uri][info.page].data;
                }
                result = GetResult(json);
            }

            if (forceRefresh && IsInternetAvailable)
            {
                lock (locker)
                {
                    ResponseCache.Remove(info.uri);
                }
            }

            if (await Task.Run(() => { lock (locker) { return !ResponseCache.ContainsKey(info.uri); } }))
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult(json);
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    ResponseCache.Add(info.uri, new Dictionary<int, (DateTime date, string data)>());
                    ResponseCache[info.uri].Add(info.page, (DateTime.Now, json));
                }
            }
            else if (await Task.Run(() => { lock (locker) { return !ResponseCache[info.uri].ContainsKey(info.page); } }))
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult(json);
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    ResponseCache[info.uri].Add(info.page, (DateTime.Now, json));
                }
            }
            else if (await Task.Run(() => { lock (locker) { return (DateTime.Now - ResponseCache[info.uri][info.page].date).TotalMinutes > 2; } }) && IsInternetAvailable)
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult(json);
                if (!result.isSucceed) { ReadCache(); }
                lock (locker)
                {
                    ResponseCache[info.uri][info.page] = (DateTime.Now, json);
                }
            }
            else
            {
                ReadCache();
            }
            return result;
        }

        public static async Task<(bool isSucceed, string result)> GetStringAsync(Uri uri, bool isBackground, bool forceRefresh = false)
        {
            string json = string.Empty;
            (int page, Uri uri) info = uri.GetPage();
            (bool isSucceed, string result) result;

            (bool isSucceed, string result) GetResult()
            {
                if (string.IsNullOrEmpty(json))
                {
                    AppUtils.ShowInAppMessage(MessageType.Message, "加载失败");
                    return (false, null);
                }
                else { return (true, json); }
            }

            if (forceRefresh)
            {
                lock (locker)
                {
                    ResponseCache.Remove(info.uri);
                }
            }

            if (await Task.Run(() => { lock (locker) { return !ResponseCache.ContainsKey(info.uri); } }))
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult();
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    ResponseCache.Add(info.uri, new Dictionary<int, (DateTime date, string data)>());
                    ResponseCache[info.uri].Add(info.page, (DateTime.Now, json));
                }
            }
            else if (await Task.Run(() => { lock (locker) { return !ResponseCache[info.uri].ContainsKey(info.page); } }))
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult();
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    ResponseCache[info.uri].Add(info.page, (DateTime.Now, json));
                }
            }
            else if (await Task.Run(() => { lock (locker) { return (DateTime.Now - ResponseCache[info.uri][info.page].date).TotalMinutes > 2; } }) && IsInternetAvailable)
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetCoolapkCookies(uri), "XMLHttpRequest", isBackground);
                result = GetResult();
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    ResponseCache[info.uri][info.page] = (DateTime.Now, json);
                }
            }
            else
            {
                lock (locker)
                {
                    json = ResponseCache[info.uri][info.page].data;
                }
                result = GetResult();
                if (!result.isSucceed) { return result; }
            }
            return result;
        }

        private static (int page, Uri uri) GetPage(this Uri uri)
        {
            Regex pageregex = new Regex(@"([&|?])page=(\d+)(\??)");
            if (pageregex.IsMatch(uri.ToString()))
            {
                int pagenum = Convert.ToInt32(pageregex.Match(uri.ToString()).Groups[2].Value);
                Uri baseuri = new Uri(pageregex.Match(uri.ToString()).Groups[3].Value == "?" ? pageregex.Replace(uri.ToString(), pageregex.Match(uri.ToString()).Groups[1].Value) : pageregex.Replace(uri.ToString(), string.Empty));
                return (pagenum, baseuri);
            }
            else
            {
                return (0, uri);
            }
        }

        private static (bool isSucceed, JToken result) GetResult(string json)
        {
            if (string.IsNullOrEmpty(json)) { return (false, null); }
            JObject o;
            try { o = JObject.Parse(json); }
            catch
            {
                AppUtils.ShowInAppMessage(MessageType.Message, "加载失败");
                return (false, null);
            }
            if (!o.TryGetValue("data", out JToken token) && o.TryGetValue("message", out JToken message))
            {
                AppUtils.ShowInAppMessage(MessageType.Message, message.ToString());
                return (false, null);
            }
            else { return (!string.IsNullOrEmpty(token.ToString()), token); }
        }

        public static string GetId(JToken token, string _idName)
        {
            return token == null
                ? string.Empty
                : (token as JObject).TryGetValue(_idName, out JToken jToken)
                    ? jToken.ToString()
                    : (token as JObject).TryGetValue("entityId", out JToken v1)
                                    ? v1.ToString()
                                    : (token as JObject).TryGetValue("id", out JToken v2) ? v2.ToString() : throw new ArgumentException(nameof(_idName));
        }
    }
}
