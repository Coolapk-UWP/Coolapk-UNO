using CoolapkUNO.Helpers;
using CoolapkUNO.ViewModels.FeedPages;
using Microsoft.Toolkit.Uwp.UI;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace CoolapkUNO.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AdaptivePage : Page
    {
        private AdaptiveViewModel Provider;

        public AdaptivePage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is AdaptiveViewModel ViewModel
                && Provider?.IsEqual(ViewModel) != true)
            {
                Provider = ViewModel;
                Provider.LoadMoreStarted += OnLoadMoreStarted;
                Provider.LoadMoreCompleted += OnLoadMoreCompleted;
                _ = Refresh(true);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Provider.LoadMoreStarted -= OnLoadMoreStarted;
            Provider.LoadMoreCompleted -= OnLoadMoreCompleted;
        }

        private static void OnLoadMoreStarted() => _ = UIHelper.ShowProgressBarAsync();

        private static void OnLoadMoreCompleted() => _ = UIHelper.HideProgressBarAsync();

        private void Page_Loaded(object sender, RoutedEventArgs e) => Provider.IsShowTitle = this.FindAscendant<Page>() is MainPage;

        public async Task Refresh(bool reset = false) => await Provider.Refresh(reset);

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => _ = Refresh(true);
    }
}
