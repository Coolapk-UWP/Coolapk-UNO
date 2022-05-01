using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoolapkUNO.WPF.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            root.Content = new global::Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new CoolapkUNO.App());
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            Border Border = sender as Border;
            double TitleHeight = WindowState == WindowState.Maximized ? 36 : 30;
            GeometryGroup GeometryGroup = new GeometryGroup();
            RectangleGeometry RectangleGeometry1 = new RectangleGeometry();
            RectangleGeometry1.Rect = new Rect(0, 0, args.NewSize.Width - 188, args.NewSize.Height);
            RectangleGeometry RectangleGeometry2 = new RectangleGeometry();
            RectangleGeometry2.Rect = new Rect(args.NewSize.Width - 188, TitleHeight, 188, args.NewSize.Height - TitleHeight);
            GeometryGroup.Children.Add(RectangleGeometry1);
            GeometryGroup.Children.Add(RectangleGeometry2);
            Border.Clip = GeometryGroup;
        }
    }
}
