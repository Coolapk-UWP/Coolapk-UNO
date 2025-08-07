using CoolapkUNO.Common;
using CoolapkUNO.Models;
using CoolapkUNO.Models.Feeds;
using CoolapkUNO.Models.Network;
using CoolapkUNO.Models.Users;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using IObjectSerializer = Microsoft.Toolkit.Helpers.IObjectSerializer;

namespace CoolapkUNO.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string Uid = nameof(Uid);
        public const string Token = nameof(Token);
        public const string TileUrl = nameof(TileUrl);
        public const string UserName = nameof(UserName);
        public const string CustomUA = nameof(CustomUA);
        public const string IsUseAPI2 = nameof(IsUseAPI2);
        public const string CustomAPI = nameof(CustomAPI);
        public const string IsFullLoad = nameof(IsFullLoad);
        public const string IsFirstRun = nameof(IsFirstRun);
        public const string IsCustomUA = nameof(IsCustomUA);
        public const string APIVersion = nameof(APIVersion);
        public const string DeviceInfo = nameof(DeviceInfo);
        public const string UpdateDate = nameof(UpdateDate);
        public const string IsNoPicsMode = nameof(IsNoPicsMode);
        public const string TokenVersion = nameof(TokenVersion);
        public const string IsUseCompositor = nameof(IsUseCompositor);
        public const string CurrentLanguage = nameof(CurrentLanguage);
        public const string IsUseMultiWindow = nameof(IsUseMultiWindow);
        public const string SelectedAppTheme = nameof(SelectedAppTheme);
        public const string ShowOtherException = nameof(ShowOtherException);
        public const string SemaphoreSlimCount = nameof(SemaphoreSlimCount);
        public const string IsDisplayOriginPicture = nameof(IsDisplayOriginPicture);
        public const string CheckUpdateWhenLaunching = nameof(CheckUpdateWhenLaunching);

        public static Type Get<Type>(string key) => LocalObject.Read<Type>(key);
        public static void Set<Type>(string key, Type value) => LocalObject.Save(key, value);
        public static void SetFile<Type>(string key, Type value) => LocalObject.CreateFileAsync(key, value);
        public static async Task<Type> GetFile<Type>(string key) => await LocalObject.ReadFileAsync<Type>(key);

        public static void SetDefaultSettings()
        {
            if (!LocalObject.KeyExists(Uid))
            {
                LocalObject.Save(Uid, string.Empty);
            }
            if (!LocalObject.KeyExists(Token))
            {
                LocalObject.Save(Token, string.Empty);
            }
            if (!LocalObject.KeyExists(TileUrl))
            {
                LocalObject.Save(TileUrl, "https://api.coolapk.com/v6/page/dataList?url=V9_HOME_TAB_FOLLOW&type=circle");
            }
            if (!LocalObject.KeyExists(UserName))
            {
                LocalObject.Save(UserName, string.Empty);
            }
            if (!LocalObject.KeyExists(CustomUA))
            {
                LocalObject.Save(CustomUA, UserAgent.Default);
            }
            if (!LocalObject.KeyExists(IsUseAPI2))
            {
                LocalObject.Save(IsUseAPI2, true);
            }
            if (!LocalObject.KeyExists(CustomAPI))
            {
                LocalObject.Save(CustomAPI, new APIVersion("9.2.2", "1905301"));
            }
            if (!LocalObject.KeyExists(IsFullLoad))
            {
                LocalObject.Save(IsFullLoad, true);
            }
            if (!LocalObject.KeyExists(IsFirstRun))
            {
                LocalObject.Save(IsFirstRun, true);
            }
            if (!LocalObject.KeyExists(IsCustomUA))
            {
                LocalObject.Save(IsCustomUA, false);
            }
            if (!LocalObject.KeyExists(APIVersion))
            {
                LocalObject.Save(APIVersion, APIVersions.V13);
            }
            if (!LocalObject.KeyExists(DeviceInfo))
            {
                LocalObject.Save(DeviceInfo, Models.Network.DeviceInfo.Default);
            }
            if (!LocalObject.KeyExists(UpdateDate))
            {
                LocalObject.Save(UpdateDate, new DateTimeOffset());
            }
            if (!LocalObject.KeyExists(IsNoPicsMode))
            {
                LocalObject.Save(IsNoPicsMode, false);
            }
            if (!LocalObject.KeyExists(TokenVersion))
            {
                LocalObject.Save(TokenVersion, Common.TokenVersion.TokenV2);
            }
            if (!LocalObject.KeyExists(IsUseCompositor))
            {
                LocalObject.Save(IsUseCompositor, true);
            }
            if (!LocalObject.KeyExists(CurrentLanguage))
            {
                LocalObject.Save(CurrentLanguage, LanguageHelper.AutoLanguageCode);
            }
            if (!LocalObject.KeyExists(IsUseMultiWindow))
            {
                LocalObject.Save(IsUseMultiWindow, true);
            }
            if (!LocalObject.KeyExists(SelectedAppTheme))
            {
                LocalObject.Save(SelectedAppTheme, ElementTheme.Default);
            }
            if (!LocalObject.KeyExists(ShowOtherException))
            {
                LocalObject.Save(ShowOtherException, true);
            }
            if (!LocalObject.KeyExists(SemaphoreSlimCount))
            {
                LocalObject.Save(SemaphoreSlimCount, Environment.ProcessorCount);
            }
            if (!LocalObject.KeyExists(IsDisplayOriginPicture))
            {
                LocalObject.Save(IsDisplayOriginPicture, false);
            }
            if (!LocalObject.KeyExists(CheckUpdateWhenLaunching))
            {
                LocalObject.Save(CheckUpdateWhenLaunching, true);
            }
        }
    }

    internal static partial class SettingsHelper
    {
        public static readonly ApplicationDataStorageHelper LocalObject = ApplicationDataStorageHelper.GetCurrent(new SystemTextJsonObjectSerializer());

        static SettingsHelper() => SetDefaultSettings();

        #region LoginChanged

        private static readonly WeakEvent<bool> actions = [];

        public static event Action<bool> LoginChanged
        {
            add => actions.Add(value);
            remove => actions.Remove(value);
        }

        public static void InvokeLoginChanged(bool args) => actions?.Invoke(args);

        #endregion

        //public static async Task<bool> Login()
        //{
        //    using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
        //    {
        //        HttpCookieManager cookieManager = filter.CookieManager;
        //        string uid = string.Empty, token = string.Empty, userName = string.Empty;
        //        foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.CoolapkUri))
        //        {
        //            switch (item.Name)
        //            {
        //                case "uid":
        //                    uid = item.Value;
        //                    break;
        //                case "username":
        //                    userName = item.Value;
        //                    break;
        //                case "token":
        //                    token = item.Value;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName) || !await RequestHelper.CheckLogin())
        //        {
        //            Logout();
        //            return false;
        //        }
        //        else
        //        {
        //            Set(Uid, uid);
        //            Set(Token, token);
        //            Set(UserName, userName);
        //            InvokeLoginChanged(uid, true);
        //            return true;
        //        }
        //    }
        //}

        //public static async Task<bool> Login(string Uid, string UserName, string Token)
        //{
        //    if (!string.IsNullOrEmpty(Uid) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Token))
        //    {
        //        using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
        //        {
        //            HttpCookieManager cookieManager = filter.CookieManager;
        //            HttpCookie uid = new HttpCookie("uid", ".coolapk.com", "/");
        //            HttpCookie username = new HttpCookie("username", ".coolapk.com", "/");
        //            HttpCookie token = new HttpCookie("token", ".coolapk.com", "/");
        //            uid.Value = Uid;
        //            username.Value = UserName;
        //            token.Value = Token;
        //            cookieManager.SetCookie(uid);
        //            cookieManager.SetCookie(username);
        //            cookieManager.SetCookie(token);
        //        }
        //        if (await RequestHelper.CheckLogin())
        //        {
        //            Set(SettingsHelper.Uid, Uid);
        //            Set(SettingsHelper.Token, Token);
        //            Set(SettingsHelper.UserName, UserName);
        //            InvokeLoginChanged(Uid, true);
        //            return true;
        //        }
        //        else
        //        {
        //            Logout();
        //            return false;
        //        }
        //    }
        //    return false;
        //}

        //public static async Task<bool> CheckLoginAsync()
        //{
        //    using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
        //    {
        //        HttpCookieManager cookieManager = filter.CookieManager;
        //        string uid = string.Empty, token = string.Empty, userName = string.Empty;
        //        foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.CoolapkUri))
        //        {
        //            switch (item.Name)
        //            {
        //                case "uid":
        //                    uid = item.Value;
        //                    break;
        //                case "username":
        //                    userName = item.Value;
        //                    break;
        //                case "token":
        //                    token = item.Value;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        return !string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userName) && await RequestHelper.CheckLogin();
        //    }
        //}

        //public static void Logout()
        //{
        //    using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
        //    {
        //        HttpCookieManager cookieManager = filter.CookieManager;
        //        foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.Base2Uri))
        //        {
        //            cookieManager.DeleteCookie(item);
        //        }
        //    }
        //    Set(Uid, string.Empty);
        //    Set(Token, string.Empty);
        //    Set(UserName, string.Empty);
        //    InvokeLoginChanged(string.Empty, false);
        //}
    }

    public class SystemTextJsonObjectSerializer : IObjectSerializer
    {
        public string Serialize<T>(T value) => value switch
        {
            int => JsonSerializer.Serialize(value, SourceGenerationContext.Default.Int32),
            bool => JsonSerializer.Serialize(value, SourceGenerationContext.Default.Boolean),
            string => JsonSerializer.Serialize(value, SourceGenerationContext.Default.String),
            UserAgent => JsonSerializer.Serialize(value, SourceGenerationContext.Default.UserAgent),
            APIVersion => JsonSerializer.Serialize(value, SourceGenerationContext.Default.APIVersion),
            DeviceInfo => JsonSerializer.Serialize(value, SourceGenerationContext.Default.DeviceInfo),
            APIVersions => JsonSerializer.Serialize(value, SourceGenerationContext.Default.APIVersions),
            TokenVersion => JsonSerializer.Serialize(value, SourceGenerationContext.Default.TokenVersion),
            ElementTheme => JsonSerializer.Serialize(value, SourceGenerationContext.Default.ElementTheme),
            DateTimeOffset => JsonSerializer.Serialize(value, SourceGenerationContext.Default.DateTimeOffset),
            _ => JsonSerializer.Serialize(value, typeof(T), SourceGenerationContext.Default)
        };

        public T Deserialize<T>([StringSyntax(StringSyntaxAttribute.Json)] string value)
        {
            if (string.IsNullOrEmpty(value)) { return default; }
            Type type = typeof(T);
            return type == typeof(int) ? Deserialize(value, SourceGenerationContext.Default.Int32)
                : type == typeof(bool) ? Deserialize(value, SourceGenerationContext.Default.Boolean)
                : type == typeof(string) ? Deserialize(value, SourceGenerationContext.Default.String)
                : type == typeof(UserAgent) ? Deserialize(value, SourceGenerationContext.Default.UserAgent)
                : type == typeof(APIVersion) ? Deserialize(value, SourceGenerationContext.Default.APIVersion)
                : type == typeof(DeviceInfo) ? Deserialize(value, SourceGenerationContext.Default.DeviceInfo)
                : type == typeof(APIVersions) ? Deserialize(value, SourceGenerationContext.Default.APIVersions)
                : type == typeof(TokenVersion) ? Deserialize(value, SourceGenerationContext.Default.TokenVersion)
                : type == typeof(ElementTheme) ? Deserialize(value, SourceGenerationContext.Default.ElementTheme)
                : type == typeof(DateTimeOffset) ? Deserialize(value, SourceGenerationContext.Default.DateTimeOffset)
                : JsonSerializer.Deserialize(value, type, SourceGenerationContext.Default) is T result ? result : default;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T Deserialize<TValue>([StringSyntax(StringSyntaxAttribute.Json)] string json, JsonTypeInfo<TValue> jsonTypeInfo) =>
                JsonSerializer.Deserialize(json, jsonTypeInfo) is T value ? value : default;
        }
    }

    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(UserAgent))]
    [JsonSerializable(typeof(APIVersion))]
    [JsonSerializable(typeof(DeviceInfo))]
    [JsonSerializable(typeof(APIVersions))]
    [JsonSerializable(typeof(TokenVersion))]
    [JsonSerializable(typeof(ElementTheme))]
    [JsonSerializable(typeof(DateTimeOffset))]
    [JsonSerializable(typeof(FeedModel))]
    [JsonSerializable(typeof(DataContainer<JsonElement>))]
    [JsonSerializable(typeof(WebDataContainer<UserInfoModel>))]
    public partial class SourceGenerationContext : JsonSerializerContext;
}
