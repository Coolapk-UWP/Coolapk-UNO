using CoolapkUNO.Common;
using CoolapkUNO.Helpers;
using System;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CoolapkUNO.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        internal bool IsExtendsTitleBar
        {
            get => CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
            set
            {
                if (IsExtendsTitleBar != value)
                {
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = value;
                    ThemeHelper.UpdateSystemCaptionButtonColors();
                }
            }
        }

        internal bool IsUseAPI2
        {
            get => SettingsHelper.Get<bool>(SettingsHelper.IsUseAPI2);
            set
            {
                if (IsUseAPI2 != value)
                {
                    SettingsHelper.Set(SettingsHelper.IsUseAPI2, value);
                    //NetworkHelper.SetRequestHeaders();
                }
            }
        }

        internal int APIVersion
        {
            get => (int)SettingsHelper.Get<APIVersion>(SettingsHelper.APIVersion) - 5;
            set
            {
                if (APIVersion != value)
                {
                    SettingsHelper.Set(SettingsHelper.APIVersion, value + 5);
                    //NetworkHelper.SetRequestHeaders();
                }
            }
        }

        internal int TokenVersion
        {
            get => (int)SettingsHelper.Get<TokenVersion>(SettingsHelper.TokenVersion);
            set
            {
                if (TokenVersion != value)
                {
                    SettingsHelper.Set(SettingsHelper.TokenVersion, value);
                    //NetworkHelper.SetRequestHeaders();
                }
            }
        }

        internal bool IsUseCompositor
        {
            get => SettingsHelper.Get<bool>(SettingsHelper.IsUseCompositor);
            set => SettingsHelper.Set(SettingsHelper.IsUseCompositor, value);
        }

        internal double SemaphoreSlimCount
        {
            get => SettingsHelper.Get<int>(SettingsHelper.SemaphoreSlimCount);
            set
            {
                if (SemaphoreSlimCount != value)
                {
                    int result = (int)Math.Floor(value);
                    SettingsHelper.Set(SettingsHelper.SemaphoreSlimCount, result);
                    //NetworkHelper.SetSemaphoreSlim(result);
                    //ImageModel.SetSemaphoreSlim(result);
                }
            }
        }

        private double progressValue = 0;
        internal double ProgressValue
        {
            get => progressValue;
            set
            {
                if (progressValue != value)
                {
                    //UIHelper.ShowProgressBar(value);
                    progressValue = value;
                }
            }
        }

        private bool isShowProgressRing = false;
        internal bool IsShowProgressRing
        {
            get => isShowProgressRing;
            set
            {
                if (isShowProgressRing != value)
                {
                    //if (value)
                    //{
                    //    UIHelper.ShowProgressBar();
                    //}
                    //else
                    //{
                    //    UIHelper.HideProgressBar();
                    //}
                    isShowProgressRing = value;
                }
            }
        }

        public TestPage() => InitializeComponent();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as FrameworkElement).Tag.ToString())
            {
                case "OutPIP":
                    _ = ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                    break;
                case "OpenURL":
                    //_ = UIHelper.OpenLinkAsync(URLTextBox.Text);
                    break;
                case "EnterPIP":
                    if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                    { _ = ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay); }
                    break;
                case "OpenBrowser":
                    //_ = Frame.Navigate(typeof(BrowserPage), new BrowserViewModel(URLTextBox.Text));
                    break;
                case "GetURLContent":
                    GetURLContent();
                    break;
                case "SettingsFlyout":
                    //if (ApiInformation.IsTypePresent("Windows.UI.ApplicationSettings.SettingsPane"))
                    //{
                    //    SettingsPane.Show();
                    //}
                    break;
                default:
                    break;
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox ComboBox = sender as ComboBox;
            switch (ComboBox.Tag.ToString())
            {
                case "Language":
                    string lang = SettingsHelper.Get<string>(SettingsHelper.CurrentLanguage);
                    lang = lang == LanguageHelper.AutoLanguageCode ? LanguageHelper.GetCurrentLanguage() : lang;
                    CultureInfo culture = new CultureInfo(lang);
                    ComboBox.SelectedItem = culture;
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ComboBox = sender as ComboBox;
            switch (ComboBox.Tag.ToString())
            {
                case "Language":
                    CultureInfo culture = ComboBox.SelectedItem as CultureInfo;
                    if (culture.Name != LanguageHelper.GetCurrentLanguage())
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
                        SettingsHelper.Set(SettingsHelper.CurrentLanguage, culture.Name);
                    }
                    else
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = string.Empty;
                        SettingsHelper.Set(SettingsHelper.CurrentLanguage, LanguageHelper.AutoLanguageCode);
                    }
                    break;
            }
        }

        private async void GetURLContent()
        {
            //Uri uri = URLTextBox.Text.ValidateAndGetUri();
            //(bool isSucceed, string result) = await RequestHelper.GetStringAsync(uri, "XMLHttpRequest");
            //if (!isSucceed)
            //{
            //    result = "网络错误";
            //}
            //ContentDialog GetJsonDialog = new ContentDialog
            //{
            //    Title = URLTextBox.Text,
            //    Content = new ScrollViewer
            //    {
            //        Content = new MarkdownTextBlock
            //        {
            //            Text = $"```json\n{result.ConvertJsonString()}\n```",
            //            Background = new SolidColorBrush(Colors.Transparent),
            //            IsTextSelectionEnabled = true
            //        },
            //        VerticalScrollMode = ScrollMode.Enabled,
            //        HorizontalScrollMode = ScrollMode.Enabled,
            //        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            //        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            //    },
            //    CloseButtonText = "好的",
            //    DefaultButton = ContentDialogButton.Close
            //};
            //_ = await GetJsonDialog.ShowAsync();
        }
    }
}
