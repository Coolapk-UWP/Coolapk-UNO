using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace CoolapkUNO.Controls
{
    public partial class TitleBarTemplateSettings
    {
        public static readonly DependencyProperty LeftPaddingColumnGridLengthProperty = DependencyProperty.Register(
           "LeftPaddingColumnGridLength",
           typeof(GridLength),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new GridLength(0)));

        public static readonly DependencyProperty IconElementProperty = DependencyProperty.Register(
           "IconElement",
           typeof(IconElement),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(default(IconSource)));

        public static readonly DependencyProperty RightPaddingColumnGridLengthProperty = DependencyProperty.Register(
           "RightPaddingColumnGridLength",
           typeof(GridLength),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new GridLength(0)));

        public static readonly DependencyProperty CustomContentMarginProperty = DependencyProperty.Register(
           "CustomContentMargin",
           typeof(Thickness),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty AutoSuggestBoxMarginProperty = DependencyProperty.Register(
           "AutoSuggestBoxMargin",
           typeof(Thickness),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty PaneFooterMarginProperty = DependencyProperty.Register(
           "PaneFooterMargin",
           typeof(Thickness),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new Thickness(0)));

        public GridLength LeftPaddingColumnGridLength
        {
            get => (GridLength)GetValue(LeftPaddingColumnGridLengthProperty);
            set => SetValue(LeftPaddingColumnGridLengthProperty, value);
        }

        public IconElement IconElement
        {
            get => (IconElement)GetValue(IconElementProperty);
            set => SetValue(IconElementProperty, value);
        }

        public GridLength RightPaddingColumnGridLength
        {
            get => (GridLength)GetValue(RightPaddingColumnGridLengthProperty);
            set => SetValue(RightPaddingColumnGridLengthProperty, value);
        }

        public Thickness CustomContentMargin
        {
            get => (Thickness)GetValue(CustomContentMarginProperty);
            set => SetValue(CustomContentMarginProperty, value);
        }

        public Thickness AutoSuggestBoxMargin
        {
            get => (Thickness)GetValue(AutoSuggestBoxMarginProperty);
            set => SetValue(AutoSuggestBoxMarginProperty, value);
        }

        public Thickness PaneFooterMargin
        {
            get => (Thickness)GetValue(PaneFooterMarginProperty);
            set => SetValue(PaneFooterMarginProperty, value);
        }
    }
}
