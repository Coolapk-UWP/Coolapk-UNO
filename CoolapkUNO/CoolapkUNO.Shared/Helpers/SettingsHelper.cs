﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace CoolapkUNO.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string Uid = "Uid";
        public const string Token = "Token";
        public const string TileUrl = "TileUrl";
        public const string UserName = "UserName";
        public const string IsUseAPI2 = "IsUseAPI2";
        public const string IsFirstRun = "IsFirstRun";
        public const string APIVersion = "APIVersion";
        public const string IsNoPicsMode = "IsNoPicsMode";
        public const string TokenVersion = "TokenVersion";
        public const string SelectedAppTheme = "SelectedAppTheme";
        public const string IsUseOldEmojiMode = "IsUseOldEmojiMode";
        public const string ShowOtherException = "ShowOtherException";
        public const string IsDisplayOriginPicture = "IsDisplayOriginPicture";
        public const string CheckUpdateWhenLuanching = "CheckUpdateWhenLuanching";

        public static Type Get<Type>(string key) => (Type)LocalSettings.Values[key];

        public static void Set(string key, object value) => LocalSettings.Values[key] = value;

        public static void SetDefaultSettings()
        {
            if (!LocalSettings.Values.ContainsKey(Uid))
            {
                LocalSettings.Values.Add(Uid, string.Empty);
            }
            if (!LocalSettings.Values.ContainsKey(Token))
            {
                LocalSettings.Values.Add(Token, string.Empty);
            }
            if (!LocalSettings.Values.ContainsKey(TileUrl))
            {
                LocalSettings.Values.Add(TileUrl, "https://api.coolapk.com/v6/page/dataList?url=V9_HOME_TAB_FOLLOW&type=circle");
            }
            if (!LocalSettings.Values.ContainsKey(UserName))
            {
                LocalSettings.Values.Add(UserName, string.Empty);
            }
            if (!LocalSettings.Values.ContainsKey(IsUseAPI2))
            {
                LocalSettings.Values.Add(IsUseAPI2, true);
            }
            if (!LocalSettings.Values.ContainsKey(IsFirstRun))
            {
                LocalSettings.Values.Add(IsFirstRun, true);
            }
            if (!LocalSettings.Values.ContainsKey(APIVersion))
            {
                LocalSettings.Values.Add(APIVersion, "V12");
            }
            if (!LocalSettings.Values.ContainsKey(IsNoPicsMode))
            {
                LocalSettings.Values.Add(IsNoPicsMode, false);
            }
            if (!LocalSettings.Values.ContainsKey(TokenVersion))
            {
                LocalSettings.Values.Add(TokenVersion, (int)Common.TokenVersion.TokenV2);
            }
            if (!LocalSettings.Values.ContainsKey(SelectedAppTheme))
            {
                LocalSettings.Values.Add(SelectedAppTheme, (int)ElementTheme.Default);
            }
            if (!LocalSettings.Values.ContainsKey(IsUseOldEmojiMode))
            {
                LocalSettings.Values.Add(IsUseOldEmojiMode, false);
            }
            if (!LocalSettings.Values.ContainsKey(ShowOtherException))
            {
                LocalSettings.Values.Add(ShowOtherException, true);
            }
            if (!LocalSettings.Values.ContainsKey(IsDisplayOriginPicture))
            {
                LocalSettings.Values.Add(IsDisplayOriginPicture, false);
            }
            if (!LocalSettings.Values.ContainsKey(CheckUpdateWhenLuanching))
            {
                LocalSettings.Values.Add(CheckUpdateWhenLuanching, true);
            }
        }
    }

    internal static partial class SettingsHelper
    {
        public static ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static readonly MetroLog.ILogManager LogManager = MetroLog.LogManagerFactory.CreateLogManager();
        public static double WindowsVersion = double.Parse($"{(ushort)((version & 0x00000000FFFF0000L) >> 16)}.{(ushort)(SettingsHelper.version & 0x000000000000FFFFL)}");

        static SettingsHelper()
        {
            SetDefaultSettings();
        }

        public static Task<bool> LoginIn() => LoginIn(Get<string>(Uid), Get<string>(UserName), Get<string>(Token));

        public static async Task<bool> LoginIn(string Uid, string UserName, string Token)
        {
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                HttpCookieManager cookieManager = filter.CookieManager;
                HttpCookie uid = new HttpCookie("uid", ".coolapk.com", "/");
                HttpCookie username = new HttpCookie("username", ".coolapk.com", "/");
                HttpCookie token = new HttpCookie("token", ".coolapk.com", "/");
                uid.Value = Uid;
                username.Value = UserName;
                token.Value = Token;
                DateTime Expires = DateTime.UtcNow.AddDays(365);
                uid.Expires = username.Expires = token.Expires = Expires;
                cookieManager.SetCookie(uid);
                cookieManager.SetCookie(username);
                cookieManager.SetCookie(token);
                return await CheckLoginInfo();
            }
        }

        public static async Task<bool> CheckLoginInfo()
        {
            try
            {
                using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
                {
                    HttpCookieManager cookieManager = filter.CookieManager;
                    string uid = string.Empty, token = string.Empty, userName = string.Empty;
                    foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.CoolapkUri))
                    {
                        switch (item.Name)
                        {
                            case "uid":
                                uid = item.Value;
                                break;

                            case "username":
                                userName = item.Value;
                                break;

                            case "token":
                                token = item.Value;
                                break;

                            default:
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName) || !await RequestHelper.CheckLogin())
                    {
                        Logout();
                        return false;
                    }
                    else
                    {
                        Set(Uid, uid);
                        Set(Token, token);
                        Set(UserName, userName);
                        return true;
                    }
                }
            }
            catch { throw; }
        }

        public static bool CheckLoginInfoFast()
        {
            try
            {
                using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
                {
                    HttpCookieManager cookieManager = filter.CookieManager;
                    string uid = string.Empty, token = string.Empty, userName = string.Empty;
                    foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.CoolapkUri))
                    {
                        switch (item.Name)
                        {
                            case "uid":
                                uid = item.Value;
                                break;

                            case "username":
                                userName = item.Value;
                                break;

                            case "token":
                                token = item.Value;
                                break;

                            default:
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName))
                    {
                        Logout();
                        return false;
                    }
                    else
                    {
                        Set(Uid, uid);
                        Set(Token, token);
                        Set(UserName, userName);
                        return true;
                    }
                }
            }
            catch { throw; }
        }

        public static void Logout()
        {
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                HttpCookieManager cookieManager = filter.CookieManager;
                foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.Base2Uri))
                {
                    cookieManager.DeleteCookie(item);
                }
            }
            Set(Uid, string.Empty);
            Set(UserName, string.Empty);
        }
    }
}
