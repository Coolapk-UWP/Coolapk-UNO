using CoolapkUNO.Helpers;
using CoolapkUNO.Networks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;

namespace CoolapkUNO.Networks
{
    public class TokenHeaderHandler : DelegatingHandler
    {
        private static readonly string guid = Guid.NewGuid().ToString();

        public CoolapkHeader Header;

        public string Token
        {
            get
            {
                var t = AppUtil.DateTimeToUnixTimeStamp(DateTime.Now);
                var hexT = "0x" + Convert.ToString((int)t, 16);
                var md5T = AppUtil.GetMD5($"{t}");
                var a = $"token://com.coolapk.market/c67ef5943784d09750dcfbb31020f0ab?{md5T}${guid}&com.coolapk.market";
                var md5A = AppUtil.GetMD5(Convert.ToBase64String(Encoding.UTF8.GetBytes(a)));
                var token = md5A + guid + hexT;
                return token;
            }
        }

        public TokenHeaderHandler(CoolapkHeader header, HttpMessageHandler innerHandler = null) : base(innerHandler ?? new HttpClientHandler())
        {
            Header = header;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("x-app-token", Token);
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("X-Sdk-Int", Header.SDKInt);
            request.Headers.Add("X-Sdk-Locale", Header.SDKLocale);
            request.Headers.Add("X-App-Id", "com.coolapk.market");
            request.Headers.Add("X-App-Version", Header.AppVersion);
            request.Headers.Add("X-App-Code", Header.AppCode);
            request.Headers.Add("X-Api-Version", Header.APIVersion);
            request.Headers.Add("X-App-Device", Header.AppDevice);
            request.Headers.Add("X-App-Mode", "universal");
            request.Headers.Add("X-App-Channel", "coolapk");
            request.Headers.Add("X-Dark-Mode", Header.DarkMode);

            var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
            var cookieManager = httpBaseProtocolFilter.CookieManager;
            var cookieCollection = cookieManager.GetCookies(request.RequestUri);
            var x = cookieCollection.ToList();
            request.Headers.Add("cookie", string.Join(";", x.Select(cookie => $"{cookie.Name}={cookie.Value}")));

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
