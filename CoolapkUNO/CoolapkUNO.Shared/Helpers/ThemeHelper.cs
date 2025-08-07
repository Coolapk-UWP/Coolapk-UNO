using CoolapkUNO.Common;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CoolapkUNO.Helpers
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private static Window CurrentApplicationWindow;

        // Keep reference so it does not get optimized/garbage collected
        public static UISettings UISettings { get; } = new UISettings();
        public static AccessibilitySettings AccessibilitySettings { get; } = new AccessibilitySettings();

        #region UISettingChanged

        private static readonly WeakEvent<UISettingChangedType> actions = [];

        public static event Action<UISettingChangedType> UISettingChanged
        {
            add => actions.Add(value);
            remove => actions.Remove(value);
        }

        internal static void InvokeUISettingChanged(UISettingChangedType value) => actions.Invoke(value);

        #endregion

        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme =>
            CurrentApplicationWindow == null
                    ? SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme)
                    : CurrentApplicationWindow.Dispatcher.HasThreadAccess
                        ? CurrentApplicationWindow.Content is FrameworkElement rootElement
                            && rootElement.RequestedTheme != ElementTheme.Default
                                ? rootElement.RequestedTheme
                                : SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme)
                        : SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get => CurrentApplicationWindow == null
                ? ElementTheme.Default
                : CurrentApplicationWindow.Dispatcher.HasThreadAccess
                    ? CurrentApplicationWindow.Content is FrameworkElement rootElement
                        ? rootElement.RequestedTheme
                        : ElementTheme.Default
                    : ElementTheme.Default;
            set
            {
                if (CurrentApplicationWindow == null) { return; }

                _ = CurrentApplicationWindow.Dispatcher.AwaitableRunAsync(() =>
                {
                    if (CurrentApplicationWindow.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = value;
                    }
                });

                SettingsHelper.Set(SettingsHelper.SelectedAppTheme, value);
                UpdateSystemCaptionButtonColors();
                InvokeUISettingChanged(IsDarkTheme() ? UISettingChangedType.DarkMode : UISettingChangedType.LightMode);
            }
        }

        static ThemeHelper()
        {
            // Registering to color changes, thus we notice when user changes theme system wide
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = Window.Current ?? App.MainWindow;
            RootTheme = SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            UpdateSystemCaptionButtonColors();
            InvokeUISettingChanged(IsDarkTheme() ? UISettingChangedType.DarkMode : UISettingChangedType.LightMode);
        }

        public static bool IsDarkTheme() =>
            Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Foreground).IsColorLight()
                    : ActualTheme == ElementTheme.Dark;

        public static bool IsDarkTheme(ElementTheme ActualTheme) =>
            Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Foreground).IsColorLight()
                    : ActualTheme == ElementTheme.Dark;

        public static bool IsColorLight(this Color color) => ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);

        public static void UpdateSystemCaptionButtonColors()
        {
            bool IsDark = IsDarkTheme();
            bool IsHighContrast = AccessibilitySettings.HighContrast;

            Color ForegroundColor = IsDark || IsHighContrast ? Colors.White : Colors.Black;
            Color BackgroundColor = IsHighContrast ? Color.FromArgb(255, 0, 0, 0) : IsDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            _ = CurrentApplicationWindow?.Dispatcher?.AwaitableRunAsync(() =>
            {
                if (UIHelper.HasStatusBar)
                {
                    StatusBar StatusBar = StatusBar.GetForCurrentView();
                    StatusBar.ForegroundColor = ForegroundColor;
                    StatusBar.BackgroundColor = BackgroundColor;
                    StatusBar.BackgroundOpacity = 0; // 透明度
                }
                else
                {
                    bool ExtendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                    TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                    TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;

#if HAS_UNO_SKIA_WPF
                    if (System.Windows.Application.Current.MainWindow is System.Windows.Window window)
                    {
                        Controls.TitleBar.SetExtendViewIntoTitleBar(window, ExtendViewIntoTitleBar);
                        Controls.TitleBar.SetForeground(window, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                        Controls.TitleBar.SetButtonForeground(window, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                        Controls.TitleBar.SetButtonHoverForeground(window, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                        Controls.TitleBar.SetButtonPressedForeground(window, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                        Controls.TitleBar.SetBackground(window, new System.Windows.Media.SolidColorBrush(BackgroundColor.ToColor()));
                        Controls.TitleBar.SetInactiveBackground(window, new System.Windows.Media.SolidColorBrush(BackgroundColor.ToColor()));
                        Controls.TitleBar.SetButtonBackground(window, new System.Windows.Media.SolidColorBrush((ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor).ToColor()));
                        Controls.TitleBar.SetButtonInactiveBackground(window, new System.Windows.Media.SolidColorBrush((ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor).ToColor()));
                    }
#endif
                }
            });
        }
    }

    public enum UISettingChangedType
    {
        LightMode,
        DarkMode,
        NoPicChanged,
    }
}
