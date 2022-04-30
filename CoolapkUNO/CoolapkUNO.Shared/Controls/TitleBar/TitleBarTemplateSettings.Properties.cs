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
        public static readonly DependencyProperty CustomColumnGridLengthProperty = DependencyProperty.Register(
           "CustomColumnGridLength",
           typeof(GridLength),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        public static readonly DependencyProperty IconElementProperty = DependencyProperty.Register(
           "IconElement",
           typeof(IconElement),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(default(IconSource)));

        public static readonly DependencyProperty TitleColumnGridLengthProperty = DependencyProperty.Register(
           "TitleColumnGridLength",
           typeof(GridLength),
           typeof(TitleBarTemplateSettings),
           new PropertyMetadata(new GridLength(1, GridUnitType.Auto)));

        public GridLength CustomColumnGridLength
        {
            get => (GridLength)GetValue(CustomColumnGridLengthProperty);
            set => SetValue(CustomColumnGridLengthProperty, value);
        }

        public IconElement IconElement
        {
            get => (IconElement)GetValue(IconElementProperty);
            set => SetValue(IconElementProperty, value);
        }

        public GridLength TitleColumnGridLength
        {
            get => (GridLength)GetValue(TitleColumnGridLengthProperty);
            set => SetValue(TitleColumnGridLengthProperty, value);
        }

    }
}
