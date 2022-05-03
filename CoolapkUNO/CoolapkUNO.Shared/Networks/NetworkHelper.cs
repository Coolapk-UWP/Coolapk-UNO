using CoolapkUNO.Exceptions;
using CoolapkUNO.Helpers;
using CoolapkUNO.Networks.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HttpClient = System.Net.Http.HttpClient;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace CoolapkUNO.Networks
{
    public static class NetworkHelper
    {
        public static readonly HttpClientHandler ClientHandler = new HttpClientHandler();
        public static readonly HttpClient Client = new HttpClient(ClientHandler);
        private static readonly string Guid = System.Guid.NewGuid().ToString();

        static NetworkHelper()
        {
            SetHeader(new CoolapkHeader());
        }

        public static void SetHeader(CoolapkHeader header)
        {
            Client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            Client.DefaultRequestHeaders.Add("X-Sdk-Int", header.SDKInt);
            Client.DefaultRequestHeaders.Add("X-Sdk-Locale", header.SDKLocale);
            Client.DefaultRequestHeaders.Add("X-App-Id", "com.coolapk.market");
            Client.DefaultRequestHeaders.Add("X-App-Version", header.AppVersion);
            Client.DefaultRequestHeaders.Add("X-App-Code", header.AppCode);
            Client.DefaultRequestHeaders.Add("X-Api-Version", header.APIVersion);
            Client.DefaultRequestHeaders.Add("X-App-Device", header.AppDevice);
            Client.DefaultRequestHeaders.Add("X-App-Mode", "universal");
            Client.DefaultRequestHeaders.Add("X-App-Channel", "coolapk");
            Client.DefaultRequestHeaders.Add("X-Dark-Mode", header.DarkMode);
        }

        public static IEnumerable<(string name, string value)> GetCoolapkCookies(Uri uri)
        {
#if WINDOWS_UWP
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                HttpCookieManager cookieManager = filter.CookieManager;
                foreach (HttpCookie item in cookieManager.GetCookies(GetHost(uri)))
                {
                    if (item.Name == "uid" ||
                        item.Name == "username" ||
                        item.Name == "token")
                    {
                        yield return (item.Name, item.Value);
                    }
                }
            }
#elif __ANDROID__
            Android.Webkit.CookieManager cookieManager = Android.Webkit.CookieManager.Instance;
            string cookie = cookieManager.GetCookie(GetHost(uri).ToString());
            foreach (var item in cookie.Split(';'))
            {
                var items = item.Split('=');
                if (items[0] == "uid" ||
                    items[0] == "username" ||
                    items[0] == "token")
                {
                    yield return (items[0], items[1]);
                }
            }
#elif __IOS__ || __MACOS__
            var nsUrl = new Foundation.NSUrl(GetHost(uri).ToString());
            foreach (var item in Foundation.NSHttpCookieStorage.SharedStorage.CookiesForUrl(nsUrl))
            {
                if (item.Name == "uid" ||
                    item.Name == "username" ||
                    item.Name == "token")
                {
                    yield return (item.Name, item.Value);
                }
            }
#else
            yield return ("uid", "0");
#endif
        }

        private static string GetCoolapkDeviceID()
        {
            Guid easId = new EasClientDeviceInformation().Id;
            string md5_easID = AppUtils.GetMD5(easId.ToString());
            string base64 = md5_easID;
            for (int i = 0; i < 5; i++)
            {
                base64 = AppUtils.GetBase64(base64);
            }
            return base64.Replace("=", "");
        }

        private static string GetCoolapkAppToken()
        {
            double t = AppUtils.DateTimeToUnixTimeStamp(DateTime.Now);
            string hex_t = "0x" + Convert.ToString((int)t, 16);
            // 时间戳加密
            string md5_t = AppUtils.GetMD5($"{t}");
            string a = $"token://com.coolapk.market/c67ef5943784d09750dcfbb31020f0ab?{md5_t}${Guid}&com.coolapk.market";
            string md5_a = AppUtils.GetMD5(AppUtils.GetBase64(a));
            string token = md5_a + Guid + hex_t;
            return token;
        }

        private static void ReplaceAppToken(this HttpRequestHeaders headers)
        {
            const string name = "X-App-Token";
            _ = headers.Remove(name);
            headers.Add(name, GetCoolapkAppToken());
        }

        private static void ReplaceRequested(this HttpRequestHeaders headers, string request)
        {
            const string name = "X-Requested-With";
            _ = headers.Remove(name);
            if (request != null) { headers.Add(name, request); }
        }

        private static void ReplaceCoolapkCookie(this CookieContainer container, IEnumerable<(string name, string value)> cookies, Uri uri)
        {
            if (cookies == null) { return; }

            foreach ((string name, string value) in cookies)
            {
                container.SetCookies(GetHost(uri), $"{name}={value}");
            }
        }

        public static Uri GetHost(Uri uri)
        {
            return new Uri("https://" + uri.Host);
        }

        private static void BeforeGetOrPost(IEnumerable<(string name, string value)> coolapkCookies, Uri uri, string request)
        {
            ClientHandler.CookieContainer.ReplaceCoolapkCookie(coolapkCookies, uri);
            Client.DefaultRequestHeaders.ReplaceAppToken();
            Client.DefaultRequestHeaders.ReplaceRequested(request);
        }

        public static async Task<string> PostAsync(Uri uri, HttpContent content, IEnumerable<(string name, string value)> coolapkCookies)
        {
            try
            {
                HttpResponseMessage response;
                BeforeGetOrPost(coolapkCookies, uri, "XMLHttpRequest");
                if (string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values["DeviceID"].ToString()))
                {
                    _ = Client.DefaultRequestHeaders.Remove("X-App-Device");
                    response = await Client.PostAsync(uri, content);
                    Client.DefaultRequestHeaders.Add("X-App-Device", GetCoolapkDeviceID());
                }
                else
                {
                    response = await Client.PostAsync(uri, content);
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch { throw; }
        }

        public static async Task<Stream> GetStreamAsync(Uri uri, IEnumerable<(string name, string value)> coolapkCookies)
        {
            try
            {
                BeforeGetOrPost(coolapkCookies, uri, "XMLHttpRequest");
                return await Client.GetStreamAsync(uri);
            }
            catch { throw; }
        }

        public static async Task<string> GetSrtingAsync(Uri uri, IEnumerable<(string name, string value)> coolapkCookies, string request, bool isBackground)
        {
            try
            {
                BeforeGetOrPost(coolapkCookies, uri, request);
                return await Client.GetStringAsync(uri);
            }
            catch (HttpRequestException e)
            {
                if (!isBackground) { AppUtils.ShowHttpExceptionMessage(e); }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 通过用户名获取UID。 </summary>
        /// <param name="name"> 要获取UID的用户名。 </param>
        public static async Task<string> GetUserIDByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new UserNameErrorException();
            }

            string str = string.Empty;
            try
            {
                str = await Client.GetStringAsync(new Uri("https://www.coolapk.com/n/" + name));
                return $"{JObject.Parse(str)["dataRow"].Value<int>("uid")}";
            }
            catch
            {
                JObject o = JObject.Parse(str);
                if (o == null) { throw; }
                else
                {
                    throw new CoolapkMessageException(o);
                }
            }
        }

        public static string ExpandShortUrl(this Uri ShortUrl)
        {
            string NativeUrl = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ShortUrl);
            try { _ = req.HaveResponse; }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.Found)
                { NativeUrl = res.Headers["Location"]; }
            }
            return NativeUrl ?? ShortUrl.ToString();
        }

        public static Uri ValidateAndGetUri(this string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString);
            }
            catch (FormatException)
            {
            }
            return uri;
        }
    }
}
