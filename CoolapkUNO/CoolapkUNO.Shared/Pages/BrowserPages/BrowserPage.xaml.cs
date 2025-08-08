using CoolapkUNO.Helpers;
using CoolapkUNO.ViewModels.BrowserPages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace CoolapkUNO.Pages.BrowserPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BrowserPage : Page
    {
        private BrowserViewModel Provider;

        public BrowserPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Frame.Navigating += OnFrameNavigating;
            if (e.Parameter is BrowserViewModel ViewModel)
            {
                Provider = ViewModel;
                if (Provider.Uri != null)
                {
                    WebView.Source = Provider.Uri;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Frame.Navigating -= OnFrameNavigating;
            WebView.Close();
        }

        private void WebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            _ = UIHelper.ShowProgressBarAsync();
        }

        private async void WebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            try
            {
                Provider.Title = sender.CoreWebView2.DocumentTitle;
                if (Provider.IsLoginPage && sender.Source.AbsoluteUri == "https://www.coolapk.com/")
                {
                    await CheckLogin();
                }
                else if (sender.Source.AbsoluteUri == UriHelper.LoginUri)
                {
                    Provider.IsLoginPage = true;
                }
            }
            catch (Exception ex)
            {
                _ = UIHelper.ShowMessageAsync(ex.Message);
            }
            finally
            {
                _ = UIHelper.HideProgressBarAsync();
            }
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs args)
        {
            if (args.NavigationMode == NavigationMode.Back && WebView.CanGoBack)
            {
                WebView.GoBack();
                args.Cancel = true;
            }
        }

        private async Task CheckLogin()
        {
            ResourceLoader loader = ResourceLoader.GetForCurrentView("BrowserPage");
            if (await SetLoginCookie() && await SettingsHelper.Login())
            {
                if (Frame.CanGoBack)
                {
                    Frame.Navigating -= OnFrameNavigating;
                    Frame.GoBack();
                }
                _ = UIHelper.ShowMessageAsync(loader.GetString("LoginSuccessfully"));
            }
            else
            {
                WebView.Source = new Uri(UriHelper.LoginUri);
                _ = UIHelper.ShowMessageAsync(loader.GetString("CannotGetToken"));
            }
        }

        public async Task<bool> SetLoginCookie()
        {
#if WINDOWS_UWP
            string Uid = string.Empty, Token = string.Empty, UserName = string.Empty;
            foreach (CoreWebView2Cookie item in await WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://coolapk.com"))
            {
                switch (item.Name)
                {
                    case "uid":
                        Uid = item.Value;
                        break;
                    case "username":
                        UserName = item.Value;
                        break;
                    case "token":
                        Token = item.Value;
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Uid) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Token))
            {
                using HttpBaseProtocolFilter filter = new();
                HttpCookieManager cookieManager = filter.CookieManager;
                HttpCookie uid = new("uid", ".coolapk.com", "/");
                HttpCookie username = new("username", ".coolapk.com", "/");
                HttpCookie token = new("token", ".coolapk.com", "/");
                uid.Value = Uid;
                username.Value = UserName;
                token.Value = Token;
                cookieManager.SetCookie(uid);
                cookieManager.SetCookie(username);
                cookieManager.SetCookie(token);
                return true;
            }
            return false;
#else
            return await Task.FromResult(true);
#endif
        }

        private void ManualLoginButton_Click(object sender, RoutedEventArgs e)
        {
            _ = UIHelper.ShowProgressBarAsync();
            //LoginDialog Dialog = new LoginDialog();
            //ContentDialogResult result = await Dialog.ShowAsync();
            //if (result == ContentDialogResult.Primary)
            //{
            //    _ = CheckLogin();
            //}
            //else
            //{
            //    UIHelper.HideProgressBar();
            //}
        }

        private void GotoSystemBrowserButton_Click(object sender, RoutedEventArgs e) => _ = Launcher.LaunchUriAsync(WebView.Source);

        private void TryLoginButton_Click(object sender, RoutedEventArgs e) => _ = CheckLogin();

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => WebView.Reload();
    }
}
