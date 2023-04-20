using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace CoolapkUNO.Controls
{
    public static class WindowHelper
    {
        private const string DefaultWindowStyleKey = "DefaultWindowStyle";
        private const string SnapWindowStyleKey = "SnapWindowStyle";

        #region UseModernWindowStyle

        public static readonly DependencyProperty UseModernWindowStyleProperty =
            DependencyProperty.RegisterAttached(
                "UseModernWindowStyle",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(OnUseModernWindowStyleChanged));

        public static bool GetUseModernWindowStyle(Window window)
        {
            return (bool)window.GetValue(UseModernWindowStyleProperty);
        }

        public static void SetUseModernWindowStyle(Window window, bool value)
        {
            window.SetValue(UseModernWindowStyleProperty, value);
        }

        private static void OnUseModernWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;

            if (DesignerProperties.GetIsInDesignMode(d))
            {
                if (d is Control control)
                {
                    if (newValue)
                    {
                        if (control.TryFindResource(DefaultWindowStyleKey) is Style style)
                        {
                            var dStyle = new Style();

                            foreach (Setter setter in style.Setters)
                            {
                                if (setter.Property == Control.BackgroundProperty ||
                                    setter.Property == Control.ForegroundProperty)
                                {
                                    dStyle.Setters.Add(setter);
                                }
                            }

                            control.Style = dStyle;
                        }
                    }
                    else
                    {
                        control.ClearValue(FrameworkElement.StyleProperty);
                    }
                }
            }
            else
            {
                var window = (Window)d;
                SetWindowStyle(window);
            }
        }

        #endregion

        #region FixMaximizedWindow

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty FixMaximizedWindowProperty =
            DependencyProperty.RegisterAttached(
                "FixMaximizedWindow",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(false, OnFixMaximizedWindowChanged));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool GetFixMaximizedWindow(Window window)
        {
            return (bool)window.GetValue(FixMaximizedWindowProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetFixMaximizedWindow(Window window, bool value)
        {
            window.SetValue(FixMaximizedWindowProperty, value);
        }

        private static void OnFixMaximizedWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if ((bool)e.NewValue)
                {
                    MaximizedWindowFixer.SetMaximizedWindowFixer(window, new MaximizedWindowFixer());
                }
                else
                {
                    window.ClearValue(MaximizedWindowFixer.MaximizedWindowFixerProperty);
                }
            }
        }

        #endregion

        public static void SetWindowStyle(Window window)
        {
            bool isModern = DependencyPropertyHelper.GetValueSource(window, UseModernWindowStyleProperty).BaseValueSource != BaseValueSource.Default && GetUseModernWindowStyle(window);

            if (isModern)
            {
                if (window.IsLoaded)
                {
                    window.RemoveTitleBar();
                }
                else
                {
                    void RemoveTitleBar(object sender, RoutedEventArgs e)
                    {
                        window.RemoveTitleBar();
                    }

                    window.Loaded -= RemoveTitleBar;
                    window.Loaded += RemoveTitleBar;
                }

                if (SnapLayout.IsSupported)
                {
                    window.SetResourceReference(FrameworkElement.StyleProperty, SnapWindowStyleKey);
                }
                else
                {
                    window.SetResourceReference(FrameworkElement.StyleProperty, DefaultWindowStyleKey);
                }
            }
            else
            {
                window.ClearValue(FrameworkElement.StyleProperty);
            }
        }

        /// <summary>
        /// Tries to remove default TitleBar from <c>hWnd</c>.
        /// </summary>
        /// <param name="window">Window to remove effect.</param>
        public static void RemoveTitleBar(this Window window)
        {
            var windowHandle = new WindowInteropHelper(window).EnsureHandle();

            if (windowHandle == IntPtr.Zero) return;

            RemoveTitleBar(windowHandle);
        }

        /// <summary>
        /// Tries to remove default TitleBar from <c>hWnd</c>.
        /// </summary>
        /// <param name="handle">Pointer to the window handle.</param>
        /// <returns><see langword="false"/> is problem occurs.</returns>
        private static bool RemoveTitleBar(IntPtr handle)
        {
            // Hide default TitleBar
            // https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window
            try
            {
                PInvoke.SetWindowLong(new HWND(handle), WINDOW_LONG_PTR_INDEX.GWL_STYLE, PInvoke.GetWindowLong(new HWND(handle), WINDOW_LONG_PTR_INDEX.GWL_STYLE) & ~0x80000);

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                return false;
            }
        }
    }
}
