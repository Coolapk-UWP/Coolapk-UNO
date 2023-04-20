using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CoolapkUNO.Controls
{
    [ContentProperty(Name = "CustomContent")]
    public sealed partial class PageHeader : UserControl
    {
        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(object),
                typeof(PageHeader),
                new PropertyMetadata(null));

        public object CustomContent
        {
            get => GetValue(CustomContentProperty);
            set => SetValue(CustomContentProperty, value);
        }

        public static readonly DependencyProperty CustomContentProperty =
            DependencyProperty.Register(
                "CustomContent",
                typeof(object),
                typeof(PageHeader),
                null);

        public double BackgroundColorOpacity
        {
            get => (double)GetValue(BackgroundColorOpacityProperty);
            set => SetValue(BackgroundColorOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorOpacityProperty =
            DependencyProperty.Register(
                "BackgroundColorOpacity",
                typeof(double),
                typeof(PageHeader),
                new PropertyMetadata(0.0));

        public double AcrylicOpacity
        {
            get => (double)GetValue(AcrylicOpacityProperty);
            set => SetValue(AcrylicOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicOpacityProperty =
            DependencyProperty.Register(
                "AcrylicOpacity",
                typeof(double),
                typeof(PageHeader),
                new PropertyMetadata(0.3));

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(
                "ShadowOpacity",
                typeof(double),
                typeof(PageHeader),
                new PropertyMetadata(0.0));

        public UIElement TitlePanel => PageTitle;

        public PageHeader() => InitializeComponent();
    }
}
