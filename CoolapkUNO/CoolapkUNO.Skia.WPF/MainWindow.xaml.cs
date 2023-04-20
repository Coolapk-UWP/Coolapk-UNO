using System;
using System.Windows;
using System.Windows.Media;

namespace CoolapkUNO.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            root.Content = new Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new CoolapkUNO.App());
            Instance = this;
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            UIElement Border = sender as UIElement;

            double TitleHeight = WindowState == WindowState.Maximized ? 36 : 30;
            double TitleWidth = Math.Max(args.NewSize.Width - 188, 0);
            
            double LeftHeight = Math.Max(args.NewSize.Height - TitleHeight, 0);
            double LeftWidth = Math.Max(args.NewSize.Width - TitleWidth, 0);

            GeometryGroup GeometryGroup = new();
            GeometryGroup.Children.Add(new RectangleGeometry
            {
                Rect = new Rect(0, 0, TitleWidth, args.NewSize.Height)
            });
            GeometryGroup.Children.Add(new RectangleGeometry
            {
                Rect = new Rect(TitleWidth, TitleHeight, LeftWidth, LeftHeight)
            });

            Border.Clip = GeometryGroup;
        }
    }
}
