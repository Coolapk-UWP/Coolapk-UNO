﻿using CoolapkUNO.Common;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
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
        public static UISettings UISettings;

        public static WeakEvent<UISettingChangedType> UISettingChanged { get; } = new WeakEvent<UISettingChangedType>();

        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                return CurrentApplicationWindow == null
                    ? SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme)
                    : CurrentApplicationWindow.Dispatcher.HasThreadAccess
                        ? CurrentApplicationWindow.Content is FrameworkElement rootElement
                            && rootElement.RequestedTheme != ElementTheme.Default
                                ? rootElement.RequestedTheme
                                : SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme)
                        : SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                return CurrentApplicationWindow == null
                    ? ElementTheme.Default
                    : CurrentApplicationWindow.Dispatcher.HasThreadAccess
                        ? CurrentApplicationWindow.Content is FrameworkElement rootElement
                            ? rootElement.RequestedTheme
                            : ElementTheme.Default
                        : ElementTheme.Default;
            }
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
                UISettingChanged.Invoke(IsDarkTheme() ? UISettingChangedType.DarkMode : UISettingChangedType.LightMode);
            }
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = Window.Current ?? App.MainWindow;
            RootTheme = SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);

            // Registering to color changes, thus we notice when user changes theme system wide
            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            UpdateSystemCaptionButtonColors();
            UISettingChanged.Invoke(IsDarkTheme() ? UISettingChangedType.DarkMode : UISettingChangedType.LightMode);
        }

        public static bool IsDarkTheme()
        {
            return Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Background) == Colors.Black
                    : ActualTheme == ElementTheme.Dark;
        }

        public static bool IsDarkTheme(ElementTheme ActualTheme)
        {
            return Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Background) == Colors.Black
                    : ActualTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            bool IsDark = IsDarkTheme();
            bool IsHighContrast = new AccessibilitySettings().HighContrast;

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
#if HAS_UNO_SKIA_WPF
                    bool ExtendViewIntoTitleBar = Controls.TitleBar.GetExtendViewIntoTitleBar(WPF.MainWindow.Instance);
                    Controls.TitleBar.SetForeground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                    Controls.TitleBar.SetButtonForeground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                    Controls.TitleBar.SetButtonHoverForeground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                    Controls.TitleBar.SetButtonPressedForeground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(ForegroundColor.ToColor()));
                    Controls.TitleBar.SetBackground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(BackgroundColor.ToColor()));
                    Controls.TitleBar.SetInactiveBackground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush(BackgroundColor.ToColor()));
                    Controls.TitleBar.SetButtonBackground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush((ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor).ToColor()));
                    Controls.TitleBar.SetButtonInactiveBackground(WPF.MainWindow.Instance, new System.Windows.Media.SolidColorBrush((ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor).ToColor()));
#else
                    bool ExtendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                    TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                    TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;
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
