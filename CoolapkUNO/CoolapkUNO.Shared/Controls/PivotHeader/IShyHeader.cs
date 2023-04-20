﻿using Windows.UI.Xaml.Controls;

namespace CoolapkUNO.Controls
{
    internal interface IShyHeader
    {
        int ShyHeaderSelectedIndex { get; set; }
        object ShyHeaderSelectedItem { get; set; }

        event SelectionChangedEventHandler ShyHeaderSelectionChanged;
    }
}
