﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoolapkUNO.ViewModels
{
    internal interface ICanComboBoxChangeSelectedIndex
    {
        List<string> ItemSource { get; }
        int ComboBoxSelectedIndex { get; }
        void SetComboBoxSelectedIndex(int value);
    }

    internal interface ICanToggleChangeSelectedIndex
    {
        bool ToggleIsOn { get; }
    }

    internal interface IViewModel
    {
        Task Refresh(bool reset);
        string Title { get; }
        double[] VerticalOffsets { get; set; }
    }
}
