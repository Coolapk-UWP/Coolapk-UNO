using CoolapkUNO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoolapkUNO.Controls
{
	[ContentProperty(Name = "CustomContent")]
	[TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
	[TemplatePart(Name = "LeftPaddingColumn", Type = typeof(ColumnDefinition))]
	[TemplatePart(Name = "RightPaddingColumn", Type = typeof(ColumnDefinition))]
	[TemplatePart(Name = "TitleText", Type = typeof(TextBlock))]
	[TemplatePart(Name = "CustomContentPresenter", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "DragRegion", Type = typeof(Grid))]
	[TemplatePart(Name = "BackButton", Type = typeof(Button))]
	[TemplatePart(Name = "Icon", Type = typeof(Viewbox))]
	public partial class TitleBar : Control
	{
		private ColumnDefinition m_leftPaddingColumn;
		private ColumnDefinition m_rightPaddingColumn;
		private Grid m_layoutRoot;
		private TextBlock m_titleTextBlock;
		private FrameworkElement m_customArea;
		private Viewbox m_icon;

		private bool m_isTitleSquished = false;
		private bool m_isIconSquished = false;

		private double m_titleWidth;
		private double m_iconWidth;

		public TitleBar()
		{
			this.DefaultStyleKey = typeof(TitleBar);

			SizeChanged += OnSizeChanged;

			var currentView = CoreApplication.GetCurrentView();
			if (currentView != null)
			{
				var coreTitleBar = currentView.TitleBar;
				if (coreTitleBar != null)
				{
					coreTitleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
					coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;
				}
			}

			var window = Window.Current;
			if (window != null)
			{
				window.Activated += OnWindowActivated;
			}

			ActualThemeChanged += (FrameworkElement sender, object args) => UpdateTheme();
		}

		protected override void OnApplyTemplate()
		{
			var currentView = CoreApplication.GetCurrentView();
			if (currentView != null)
			{
				var coreTitleBar = currentView.TitleBar;
				if (coreTitleBar != null)
				{
					coreTitleBar.ExtendViewIntoTitleBar = true;
				}
			}

			m_layoutRoot = (Grid)GetTemplateChild("LayoutRoot");
			m_leftPaddingColumn = (ColumnDefinition)GetTemplateChild("LeftPaddingColumn");
			m_rightPaddingColumn = (ColumnDefinition)GetTemplateChild("RightPaddingColumn");

			m_icon = (Viewbox)GetTemplateChild("Icon");
			m_titleTextBlock = (TextBlock)GetTemplateChild("TitleText");
			m_customArea = (FrameworkElement)GetTemplateChild("CustomContentPresenter");

			var window = Window.Current;
			if (window != null)
			{
				var dragRegion = (Grid)GetTemplateChild("DragRegion");
				if (dragRegion != null)
				{
					window.SetTitleBar(dragRegion);
				}
				else
				{
					window.SetTitleBar(null);
				}
			}

			var backButton = (Button)GetTemplateChild("BackButton");
			if (backButton != null)
			{
				backButton.Click += OnBackButtonClick;

				// Do localization for the back button
				if (string.IsNullOrEmpty(AutomationProperties.GetName(backButton)))
				{
					var backButtonName = ResourceLoader.GetForViewIndependentUse().GetString("NavigationBackButtonName");
					AutomationProperties.SetName(backButton, backButtonName);
				}

				// Setup the tooltip for the back button
				var tooltip = new ToolTip();
				var backButtonTooltipText = ResourceLoader.GetForViewIndependentUse().GetString("NavigationBackButtonToolTip");
				tooltip.Content = backButtonTooltipText;
				ToolTipService.SetToolTip(backButton, tooltip);
			}

			UpdateVisibility();
			UpdateHeight();
			UpdatePadding();
			UpdateIcon();
			UpdateBackButton();
			UpdateTheme();
			UpdateTitle();

			base.OnApplyTemplate();
		}

		public void OnBackButtonClick(object sender, RoutedEventArgs args)
		{
			BackRequested?.Invoke(this, null);
		}

		public void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			UpdateIcon();
		}

		public void OnIsBackButtonVisiblePropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			UpdateBackButton();
		}

		public void OnCustomContentPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			UpdateHeight();
		}

		public void OnTitlePropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			UpdateTitle();
		}

		public void OnSizeChanged(object sender, SizeChangedEventArgs args)
		{
			var titleTextBlock = m_titleTextBlock;
			var customArea = m_customArea;
			if (titleTextBlock != null && !string.IsNullOrEmpty(titleTextBlock.Text) && customArea != null)
			{
				if (m_isTitleSquished)
				{
					var icon = m_icon;
					var source = IconSource;
					if (icon != null && source != null)
					{
						if (m_isIconSquished)
						{
							if (customArea.DesiredSize.Width + m_iconWidth + 16 < customArea.ActualWidth)
							{
								VisualStateManager.GoToState(this, "IconVisible", true);
								m_isIconSquished = false;

								if (customArea.DesiredSize.Width + m_iconWidth + m_titleWidth + 32 < customArea.ActualWidth)
								{
									VisualStateManager.GoToState(this, "TitleTextVisible", true);
									m_isTitleSquished = false;
								}
							}
						}
						else
						{
							if (customArea.DesiredSize.Width + m_titleWidth + 16 < customArea.ActualWidth)
							{
								VisualStateManager.GoToState(this, "TitleTextVisible", true);
								m_isTitleSquished = false;
							}

							if (!m_isIconSquished && customArea.DesiredSize.Width >= customArea.ActualWidth)
							{
								VisualStateManager.GoToState(this, "IconCollapsed", true);
								m_iconWidth = titleTextBlock.ActualWidth;
								m_isIconSquished = true;
							}
						}
					}
					else
					{
						if (customArea.DesiredSize.Width + m_titleWidth + 16 < customArea.ActualWidth)
						{
							VisualStateManager.GoToState(this, "TitleTextVisible", true);
							m_isTitleSquished = false;
						}
					}
				}
				else
				{
					if (!m_isTitleSquished && customArea.DesiredSize.Width >= customArea.ActualWidth)
					{
						VisualStateManager.GoToState(this, "TitleTextCollapsed", true);
						m_titleWidth = titleTextBlock.ActualWidth;
						m_isTitleSquished = true;
					}
				}
			}
		}

		public void OnWindowActivated(object sender, WindowActivatedEventArgs args)
		{
			VisualStateManager.GoToState(this, (args.WindowActivationState == CoreWindowActivationState.Deactivated) ? "Deactivated" : "Activated", false);
		}

		public void OnTitleBarMetricsChanged(object UnnamedParameter, object UnnamedParameter2)
		{
			UpdatePadding();
		}

		public void OnTitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object UnnamedParameter)
		{
			UpdateVisibility();
		}

		public void UpdateIcon()
		{
			var templateSettings = TemplateSettings;
			var source = IconSource;
			if (source != null)
			{
				templateSettings.IconElement = SharedHelpers.MakeIconElementFrom(source);
				VisualStateManager.GoToState(this, "IconVisible", false);
			}
			else
			{
				templateSettings.IconElement = null;
				VisualStateManager.GoToState(this, "IconCollapsed", false);
			}
		}

		public void UpdateBackButton()
		{
			VisualStateManager.GoToState(this, IsBackButtonVisible ? "BackButtonVisible" : "BackButtonCollapsed", false);
		}

		public void UpdateVisibility()
		{
			var currentView = CoreApplication.GetCurrentView();
			if (currentView != null)
			{
				var coreTitleBar = currentView.TitleBar;
				if (coreTitleBar != null)
				{
					VisualStateManager.GoToState(this, coreTitleBar.IsVisible ? "TitleBarVisible" : "TitleBarCollapsed", false);
				}
			}
		}

		public void UpdateHeight()
		{
			VisualStateManager.GoToState(this, (CustomContent == null) ? "CompactHeight" : "ExpandedHeight", false);
		}

		public void UpdatePadding()
		{
			var currentView = CoreApplication.GetCurrentView();
			if (currentView != null)
			{
				var coreTitleBar = currentView.TitleBar;
				if (coreTitleBar != null)
				{
					var leftColumn = m_leftPaddingColumn;
					if (m_leftPaddingColumn != null)
					{
						leftColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
					}

					var rightColumn = m_rightPaddingColumn;
					if (rightColumn != null)
					{
#if HAS_UNO_SKIA_WPF
						rightColumn.Width = new GridLength(188);
#else
						rightColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
#endif
					}
				}
			}
		}

		public void UpdateTheme()
		{
			var appView = ApplicationView.GetForCurrentView();
			if (appView != null)
			{
				var titleBar = appView.TitleBar;
				if (titleBar != null)
				{
					ResourceDictionary ResourceDictionary = new ResourceDictionary();
					ResourceDictionary.Source = new Uri("ms-appx:///Controls/TitleBar/TitleBar_themeresources.xaml");

					// rest colors
					var buttonForegroundColor = (Color)ResourceDictionary["TitleBarButtonForegroundColor"];
					titleBar.ButtonForegroundColor = buttonForegroundColor;

					var buttonBackgroundColor = (Color)ResourceDictionary["TitleBarButtonBackgroundColor"];
					titleBar.ButtonBackgroundColor = buttonBackgroundColor;
					titleBar.ButtonInactiveBackgroundColor = buttonBackgroundColor;

					// hover colors
					var buttonHoverForegroundColor = (Color)ResourceDictionary["TitleBarButtonHoverForegroundColor"];
					titleBar.ButtonHoverForegroundColor = buttonHoverForegroundColor;

					var buttonHoverBackgroundColor = (Color)ResourceDictionary["TitleBarButtonHoverBackgroundColor"];
					titleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundColor;

					// pressed colors
					var buttonPressedForegroundColor = (Color)ResourceDictionary["TitleBarButtonPressedForegroundColor"];
					titleBar.ButtonPressedForegroundColor = buttonPressedForegroundColor;

					var buttonPressedBackgroundColor = (Color)ResourceDictionary["TitleBarButtonPressedBackgroundColor"];
					titleBar.ButtonPressedBackgroundColor = buttonPressedBackgroundColor;

					// inactive foreground
					var buttonInactiveForegroundColor = (Color)ResourceDictionary["TitleBarButtonInactiveForegroundColor"];
					titleBar.ButtonInactiveForegroundColor = buttonInactiveForegroundColor;
				}
			}
		}

		public void UpdateTitle()
		{
			string titleText = Title;
			if (string.IsNullOrEmpty(titleText))
			{
				VisualStateManager.GoToState(this, "TitleTextCollapsed", false);
			}
			else
			{
				VisualStateManager.GoToState(this, "TitleTextVisible", false);
			}
		}
	}
}
