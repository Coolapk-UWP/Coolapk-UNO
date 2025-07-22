using CoolapkUNO.Controls;
using System.Windows;
using Uno.UI.Runtime.Skia.Wpf;

namespace CoolapkUNO.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            WpfHost host = new(Dispatcher, () => new CoolapkUNO.App());
            host.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (MainWindow is Window window)
            {
                if (!WindowHelper.GetUseModernWindowStyle(window))
                {
                    WindowHelper.SetUseModernWindowStyle(window, true);
                    TitleBar.SetExtendViewIntoTitleBar(window, true);
                }
            }
        }
    }
}
