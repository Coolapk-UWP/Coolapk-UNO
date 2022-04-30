using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using BitmapIconSource = Microsoft.UI.Xaml.Controls.BitmapIconSource;
using FontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;
using IconSource = Microsoft.UI.Xaml.Controls.IconSource;
using PathIconSource = Microsoft.UI.Xaml.Controls.PathIconSource;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;

namespace CoolapkUNO.Helpers
{
	public static class SharedHelpers
	{
		public static IconElement MakeIconElementFrom(IconSource iconSource)
		{
			if (iconSource is FontIconSource fontIconSource)
			{
				FontIcon fontIcon = new FontIcon();

				fontIcon.Glyph = fontIconSource.Glyph;
				fontIcon.FontSize = fontIconSource.FontSize;
				var newForeground = fontIconSource.Foreground;
				if (newForeground != null)
				{
					fontIcon.Foreground = newForeground;
				}

				if (fontIconSource.FontFamily != null)
				{
					fontIcon.FontFamily = fontIconSource.FontFamily;
				}

				fontIcon.FontWeight = fontIconSource.FontWeight;
				fontIcon.FontStyle = fontIconSource.FontStyle;
				fontIcon.IsTextScaleFactorEnabled = fontIconSource.IsTextScaleFactorEnabled;
				fontIcon.MirroredWhenRightToLeft = fontIconSource.MirroredWhenRightToLeft;

				return fontIcon;
			}
			else if (iconSource is SymbolIconSource symbolIconSource)
			{
				SymbolIcon symbolIcon = new SymbolIcon();
				symbolIcon.Symbol = symbolIconSource.Symbol;
				var newForeground = symbolIconSource.Foreground;
				if (newForeground != null)
				{
					symbolIcon.Foreground = newForeground;
				}
				return symbolIcon;
			}
			else if (iconSource is BitmapIconSource bitmapIconSource)
			{
				BitmapIcon bitmapIcon = new BitmapIcon();

				if (bitmapIconSource.UriSource != null)
				{
					bitmapIcon.UriSource = bitmapIconSource.UriSource;
				}

				if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.BitmapIcon", "ShowAsMonochrome"))
				{
					bitmapIcon.ShowAsMonochrome = bitmapIconSource.ShowAsMonochrome;
				}
				var newForeground = bitmapIconSource.Foreground;
				if (newForeground != null)
				{
					bitmapIcon.Foreground = newForeground;
				}
				return bitmapIcon;
			}
			else if (iconSource is ImageIconSource imageIconSource)
			{
				ImageIcon imageIcon = new ImageIcon();
				var imageSource = imageIconSource.ImageSource;
				if (imageSource != null)
				{
					imageIcon.Source = imageSource;
				}
				var newForeground = imageIconSource.Foreground;
				if (newForeground != null)
				{
					imageIcon.Foreground = newForeground;
				}
				return imageIcon;
			}
			else if (iconSource is PathIconSource pathIconSource)
			{
				PathIcon pathIcon = new PathIcon();

				if (pathIconSource.Data != null)
				{
					pathIcon.Data = pathIconSource.Data;
				}
				var newForeground = pathIconSource.Foreground;
				if (newForeground != null)
				{
					pathIcon.Foreground = newForeground;
				}
				return pathIcon;
			}
			else if (iconSource is AnimatedIconSource animatedIconSource)
			{
				AnimatedIcon animatedIcon = new AnimatedIcon();
				var source = animatedIconSource.Source;
				if (source != null)
				{
					animatedIcon.Source = source;
				}
				var fallbackIconSource = animatedIconSource.FallbackIconSource;
				if (fallbackIconSource != null)
				{
					animatedIcon.FallbackIconSource = fallbackIconSource;
				}
				var newForeground = animatedIconSource.Foreground;
				if (newForeground != null)
				{
					animatedIcon.Foreground = newForeground;
				}
				return animatedIcon;
			}
			return null;
		}
	}
}
