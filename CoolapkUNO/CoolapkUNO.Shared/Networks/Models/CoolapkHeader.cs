using System;
using System.Collections.Generic;
using System.Text;

namespace CoolapkUNO.Networks.Models
{
    public class CoolapkHeader
    {
        public string SDKInt { get; set; } = "30";
        public string SDKLocale { get; set; } = "zh-CN";
        public string DarkMode { get; set; } = "0";
        public string AppVersion { get; set; } = "9.2.2";
        public string AppCode { get; set; } = "1905301";
        public string APIVersion { get; set; } = "9";
        public string AppDevice { get; set; }
        public string SystemName { get; set; } = "Windows NT";
        public string SystemVersion { get; set; } = "10.0";
        public string FullSystemVersion { get; set; } = "10.0.22610.1";
        public string Architecture { get; set; } = "x64";
        public string Manufacturer { get; set; } = "OnePlus";
        public string ProductName { get; set; } = "GM1910";

        public string UserAgent => $"Dalvik/2.1.0 ({SystemName} {SystemVersion}; Win32; {Architecture}; WebView/3.0) (#Build; {Manufacturer}; {ProductName}; CoolapkUNO; {FullSystemVersion}) +CoolMarket/{AppVersion}-{AppCode}-universal";
    }
}
