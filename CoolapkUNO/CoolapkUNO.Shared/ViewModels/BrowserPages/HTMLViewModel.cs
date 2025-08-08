using CoolapkUNO.Common;
using CoolapkUNO.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;

namespace CoolapkUNO.ViewModels.BrowserPages
{
    public class HTMLViewModel : IViewModel
    {
        private readonly Uri uri;

        public CoreDispatcher Dispatcher { get; }

        private string title;
        public string Title
        {
            get => title;
            private set => SetProperty(ref title, value);
        }

        private string html;
        public string HTML
        {
            get => html;
            private set => SetProperty(ref html, value);
        }

        private string rawHTML;
        public string RawHTML
        {
            get => rawHTML;
            private set
            {
                if (rawHTML != value)
                {
                    rawHTML = value;
                    _ = GetHtmlAsync(value, title);
                    RaisePropertyChangedEvent();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void RaisePropertyChangedEvent([CallerMemberName] string name = null)
        {
            if (name != null)
            {
                await Dispatcher.ResumeForegroundAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void SetProperty<TProperty>(ref TProperty property, TProperty value, [CallerMemberName] string name = null)
        {
            if (property == null ? value != null : !property.Equals(value))
            {
                property = value;
                RaisePropertyChangedEvent(name);
            }
        }

        public HTMLViewModel(string url, CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            uri = url.TryGetUri();
            ThemeHelper.UISettingChanged += OnUISettingChanged;
        }

        ~HTMLViewModel()
        {
            ThemeHelper.UISettingChanged -= OnUISettingChanged;
        }

        private void OnUISettingChanged(UISettingChangedType mode)
        {
            switch (mode)
            {
                case UISettingChangedType.LightMode:
                    _ = GetHtmlAsync(RawHTML, "Light");
                    break;
                case UISettingChangedType.DarkMode:
                    _ = GetHtmlAsync(RawHTML, "Dark");
                    break;
                case UISettingChangedType.NoPicChanged:
                    break;
            }
        }

        public async Task Refresh(bool reset)
        {
            if (uri != null)
            {
                await LoadHtmlAsync(uri).ConfigureAwait(false);
            }
        }

        bool IViewModel.IsEqual(IViewModel other) => other is HTMLViewModel model && IsEqual(model);
        public bool IsEqual(HTMLViewModel other) => uri == other.uri;

        private async Task LoadHtmlAsync(Uri uri)
        {
            _ = UIHelper.ShowProgressBarAsync();
            try
            {
                (bool isSucceed, string result) = await RequestHelper.GetStringAsync(uri, NetworkHelper.XMLHttpRequest).ConfigureAwait(false);
                if (isSucceed)
                {
                    JsonDocument document = JsonDocument.Parse(result);
                    JsonElement element = document.RootElement;

                    if (element.TryGetProperty("title", out JsonElement title))
                    {
                        Title = title.GetString();
                    }

                    if (element.TryGetProperty("html", out JsonElement html))
                    {
                        RawHTML = html.GetString();
                    }
                    else if (element.TryGetProperty("description", out JsonElement description))
                    {
                        RawHTML = description.GetString();
                    }
                    else
                    {
                        (isSucceed, result) = await RequestHelper.GetStringAsync(uri).ConfigureAwait(false);
                        if (isSucceed && !string.IsNullOrWhiteSpace(result))
                        {
                            //HtmlDocument doc = new HtmlDocument();
                            //doc.LoadHtml(result);
                            //string content = doc.DocumentNode.ChildNodes.FindFirst("html")?.ChildNodes.FindFirst("body")?.InnerHtml;
                            //if (!string.IsNullOrEmpty(content))
                            //{
                            //    RawHTML = content;
                            //    return;
                            //}
                        }
                    }
                }
                RawHTML = "<h1>网络错误</h1>";
            }
            finally
            {
                _ = UIHelper.HideProgressBarAsync();
            }
        }

        public async Task GetHtmlAsync(string html, string title) => await GetHtmlAsync(html, title, ThemeHelper.IsDarkTheme() ? "Dark" : "Light").ConfigureAwait(false);

        public async Task GetHtmlAsync(string html, string title, string theme)
        {
            StorageFile indexFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/WebView/HTMLView.html"));
            string index = await FileIO.ReadTextAsync(indexFile);
            HTML = index.Replace("{{RenderTheme}}", theme).Replace("{{Title}}", title).Replace("{{HTMLBody}}", html);
        }
    }
}
