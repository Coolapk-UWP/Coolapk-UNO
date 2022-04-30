﻿using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using IObjectSerializer = Microsoft.Toolkit.Helpers.IObjectSerializer;

namespace CoolapkUNO.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string IsDarkMode = "IsDarkMode";
        public const string IsFirstRun = "IsFirstRun";
        public const string CurrentLanguage = "CurrentLanguage";
        public const string IsBackgroundColorFollowSystem = "IsBackgroundColorFollowSystem";

        public static Type Get<Type>(string key) => LocalObject.Read<Type>(key);
        public static void Set(string key, object value) => LocalObject.Save(key, value);
        public static void SetFile(string key, object value) => LocalObject.CreateFileAsync(key, value);
        public static async Task<Type> GetFile<Type>(string key) => await LocalObject.ReadFileAsync<Type>(key);

        public static void SetDefaultSettings()
        {
            if (!LocalObject.KeyExists(IsFirstRun))
            {
                LocalObject.Save(IsFirstRun, true);
            }
            if (!LocalObject.KeyExists(IsDarkMode))
            {
                LocalObject.Save(IsDarkMode, false);
            }
            if (!LocalObject.KeyExists(CurrentLanguage))
            {
                LocalObject.Save(CurrentLanguage, LanguageHelper.AutoLanguageCode);
            }
            if (!LocalObject.KeyExists(IsBackgroundColorFollowSystem))
            {
                LocalObject.Save(IsBackgroundColorFollowSystem, true);
            }
        }
    }

    public enum UISettingChangedType
    {
        LightMode,
        DarkMode,
        NoPicChanged,
    }

    internal static partial class SettingsHelper
    {
        public static readonly UISettings UISettings = new UISettings();
        public static OSVersion OperatingSystemVersion => SystemInformation.Instance.OperatingSystemVersion;
        private static readonly ApplicationDataStorageHelper LocalObject = ApplicationDataStorageHelper.GetCurrent(new SystemTextJsonObjectSerializer());
        public static ElementTheme Theme => Get<bool>("IsBackgroundColorFollowSystem") ? ElementTheme.Default : (Get<bool>("IsDarkMode") ? ElementTheme.Dark : ElementTheme.Light);

        static SettingsHelper()
        {
            SetDefaultSettings();
            UISettings.ColorValuesChanged += SetBackgroundTheme;
        }

        private static void SetBackgroundTheme(UISettings sender, object args)
        {
            if (Get<bool>(IsBackgroundColorFollowSystem))
            {
                bool value = sender.GetColorValue(UIColorType.Background) == Colors.Black;
                Set(IsDarkMode, value);
                _ = UIHelper.DispatcherQueue?.EnqueueAsync(() => UIHelper.CheckTheme());
            }
        }
    }

    public class SystemTextJsonObjectSerializer : IObjectSerializer
    {
        // Specify your serialization settings
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore };

        string IObjectSerializer.Serialize<T>(T value) => JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, settings);

        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
    }
}
