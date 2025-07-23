using CoolapkUNO.Common;
using CoolapkUNO.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

namespace CoolapkUNO.Helpers
{
    internal static partial class UIHelper
    {
        public const int Duration = 3000;
        public static bool IsShowingProgressBar, IsShowingMessage;
        public static bool HasStatusBar => ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        private static readonly List<string> MessageList = new List<string>();
    }

    internal static partial class UIHelper
    {
        public static async void ShowProgressBar()
        {
            await MainPage.Dispatcher.ResumeForegroundAsync();
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                MainPage.HideProgressBar();
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
                await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                MainPage.ShowProgressBar();
            }
        }

        public static async void ShowProgressBar(double value = 0)
        {
            await MainPage.Dispatcher.ResumeForegroundAsync();
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                MainPage.HideProgressBar();
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = value * 0.01;
                await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                MainPage.ShowProgressBar(value);
            }
        }

        public static async void PausedProgressBar()
        {
            await MainPage.Dispatcher.ResumeForegroundAsync();
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage.PausedProgressBar();
        }

        public static async void ErrorProgressBar()
        {
            await MainPage.Dispatcher.ResumeForegroundAsync();
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage.ErrorProgressBar();
        }

        public static async void HideProgressBar()
        {
            await MainPage.Dispatcher.ResumeForegroundAsync();
            IsShowingProgressBar = false;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage.HideProgressBar();
        }

        public static async void ShowMessage(string message)
        {
            MessageList.Add(message);
            if (!IsShowingMessage)
            {
                IsShowingMessage = true;
                await MainPage.Dispatcher.ResumeForegroundAsync();
                while (MessageList.Count > 0)
                {
                    if (HasStatusBar)
                    {
                        StatusBar statusBar = StatusBar.GetForCurrentView();
                        if (!string.IsNullOrEmpty(MessageList[0]))
                        {
                            statusBar.ProgressIndicator.Text = $"[{MessageList.Count}] {MessageList[0].Replace("\n", " ")}";
                            statusBar.ProgressIndicator.ProgressValue = IsShowingProgressBar ? null : (double?)0;
                            await statusBar.ProgressIndicator.ShowAsync();
                            await Task.Delay(Duration);
                        }
                        MessageList.RemoveAt(0);
                        if (MessageList.Count == 0 && !IsShowingProgressBar) { await statusBar.ProgressIndicator.HideAsync(); }
                        statusBar.ProgressIndicator.Text = string.Empty;
                    }
                    else if (MainPage != null)
                    {
                        if (!string.IsNullOrEmpty(MessageList[0]))
                        {
                            string messages = $"[{MessageList.Count}] {MessageList[0].Replace("\n", " ")}";
                            MainPage.ShowMessage(messages);
                            await Task.Delay(Duration);
                        }
                        MessageList.RemoveAt(0);
                        if (MessageList.Count == 0)
                        {
                            MainPage.ShowMessage();
                        }
                    }
                }
                IsShowingMessage = false;
            }
        }

        public static string ExceptionToMessage(this Exception ex)
        {
            StringBuilder builder = new();
            builder.Append('\n');
            if (!string.IsNullOrWhiteSpace(ex.Message)) { builder.AppendLine($"Message: {ex.Message}"); }
            builder.AppendLine($"HResult: {ex.HResult} (0x{ex.HResult:X})");
            if (!string.IsNullOrWhiteSpace(ex.StackTrace)) { builder.AppendLine(ex.StackTrace); }
            if (!string.IsNullOrWhiteSpace(ex.HelpLink)) { builder.Append($"HelperLink: {ex.HelpLink}"); }
            return builder.ToString();
        }
    }

    internal static partial class UIHelper
    {
        public static MainPage MainPage;
    }
}
