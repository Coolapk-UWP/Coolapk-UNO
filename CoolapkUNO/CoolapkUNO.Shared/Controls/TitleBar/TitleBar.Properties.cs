using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using IconSource = Microsoft.UI.Xaml.Controls.IconSource;

namespace CoolapkUNO.Controls
{
    public partial class TitleBar
    {
        public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(
           "CustomContent",
           typeof(object),
           typeof(TitleBar),
           new PropertyMetadata(default, OnCustomContentPropertyChanged));

        public static readonly DependencyProperty AutoSuggestBoxProperty = DependencyProperty.Register(
           "AutoSuggestBox",
           typeof(AutoSuggestBox),
           typeof(TitleBar),
           new PropertyMetadata(default, OnCustomContentPropertyChanged));

        public static readonly DependencyProperty PaneFooterProperty = DependencyProperty.Register(
           "PaneFooter",
           typeof(object),
           typeof(TitleBar),
           new PropertyMetadata(default, OnCustomContentPropertyChanged));

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
           "IconSource",
           typeof(IconSource),
           typeof(TitleBar),
           new PropertyMetadata(default(IconSource), OnIconSourcePropertyChanged));

        public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register(
           "IsBackButtonVisible",
           typeof(bool),
           typeof(TitleBar),
           new PropertyMetadata(default(bool), OnIsBackButtonVisiblePropertyChanged));

        public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.Register(
           "IsBackEnabled",
           typeof(bool),
           typeof(TitleBar),
           new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register(
           "TemplateSettings",
           typeof(TitleBarTemplateSettings),
           typeof(TitleBar),
           new PropertyMetadata(new TitleBarTemplateSettings()));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
           "Title",
           typeof(string),
           typeof(TitleBar),
           new PropertyMetadata(default(string), OnTitlePropertyChanged));

        public object CustomContent
        {
            get => GetValue(CustomContentProperty);
            set => SetValue(CustomContentProperty, value);
        }

        public object AutoSuggestBox
        {
            get => (AutoSuggestBox)GetValue(AutoSuggestBoxProperty);
            set => SetValue(AutoSuggestBoxProperty, value);
        }

        public object PaneFooter
        {
            get => GetValue(PaneFooterProperty);
            set => SetValue(PaneFooterProperty, value);
        }

        public IconSource IconSource
        {
            get => (IconSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public bool IsBackButtonVisible
        {
            get => (bool)GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        public bool IsBackEnabled
        {
            get => (bool)GetValue(IsBackEnabledProperty);
            set => SetValue(IsBackEnabledProperty, value);
        }

        public TitleBarTemplateSettings TemplateSettings
        {
            get => (TitleBarTemplateSettings)GetValue(TemplateSettingsProperty);
            set => SetValue(TemplateSettingsProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public event TypedEventHandler<TitleBar, object> BackRequested;

        private static void OnCustomContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TitleBar)d).OnCustomContentPropertyChanged(e);
        }

        private static void OnIconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TitleBar)d).OnIconSourcePropertyChanged(e);
        }

        private static void OnIsBackButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TitleBar)d).OnIsBackButtonVisiblePropertyChanged(e);
        }

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TitleBar)d).OnTitlePropertyChanged(e);
        }
    }
}
