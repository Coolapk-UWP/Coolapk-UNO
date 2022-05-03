using System;
using System.Collections.Generic;
using System.Text;

namespace CoolapkUNO.Networks
{
    /// <summary>
    /// PageAPIs
    /// </summary>
    public static partial class CoolapkAPIs
    {
        public static string IndexTabs = "/v6/main/init";
        public static string IndexPage = "/v6{0}{1}page={2}";
    }

    public static partial class CoolapkAPIs
    {
        public static readonly Uri CoolapkUri = new Uri("https://coolapk.com");
        public static readonly Uri BaseUri = new Uri("https://api.coolapk.com");
        public static readonly Uri ITHomeUri = new Uri("https://qapi.ithome.com");
        public static readonly Uri DevUri = new Uri("https://developer.coolapk.com");
        public static readonly Uri BilibiliUri = new Uri("https://api.vc.bilibili.com");

        public static Uri GetUri(this string api, params object[] args)
        {
            string u = string.Format(api, args);
            return new Uri(BaseUri, u);
        }
    }
}
