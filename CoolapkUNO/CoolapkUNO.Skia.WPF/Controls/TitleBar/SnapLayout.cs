﻿using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Win32;

namespace CoolapkUNO.Controls
{
    /// <summary>
    /// Brings the Snap Layout functionality from Windows 11 to a custom <see cref="TitleBar"/>.
    /// </summary>
    internal sealed class SnapLayout
    {
        public SolidColorBrush DefaultButtonBackground { get; set; } = Brushes.Transparent;

        private bool _isButtonFocused;

        private bool _isButtonClicked;

        private double _dpiScale;

        private Button _button;

        private SolidColorBrush _hoverColor;

        public void Register(Button button)
        {
            _isButtonFocused = false;
            _button = button;

            HwndSource hwnd = (HwndSource)PresentationSource.FromVisual(button);

            _dpiScale = VisualTreeHelper.GetDpi(button).DpiScaleX;

            SetHoverColor();

            hwnd?.AddHook(HwndSourceHook);
        }

        public static bool IsSupported => Environment.OSVersion.Version.Build >= 21390;

        /// <summary>
        /// Represents the method that handles Win32 window messages.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="uMsg">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">The message's lParam value.</param>
        /// <param name="handled">A value that indicates whether the message was handled. Set the value to <see langword="true"/> if the message was handled; otherwise, <see langword="false"/>.</param>
        /// <returns>The appropriate return value depends on the particular message. See the message documentation details for the Win32 message being handled.</returns>
        private IntPtr HwndSourceHook(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // TODO: This whole class is one big todo

            uint mouseNotification = (uint)uMsg;

            switch (mouseNotification)
            {
                case PInvoke.WM_NCLBUTTONDOWN:
                    if (IsOverButton(wParam, lParam))
                    {
                        _isButtonClicked = true;
                        handled = true;
                    }
                    break;

                case PInvoke.WM_NCMOUSELEAVE:
                    DefocusButton();
                    break;

                case PInvoke.WM_NCLBUTTONUP:
                    if (_isButtonClicked)
                    {
                        if (IsOverButton(wParam, lParam))
                        {
                            RaiseButtonClick();
                        }
                        _isButtonClicked = false;
                    }
                    break;

                case PInvoke.WM_NCHITTEST:
                    if (IsOverButton(wParam, lParam))
                    {
                        FocusButton();
                        handled = true;
                    }
                    else
                    {
                        DefocusButton();
                    }
                    return new IntPtr(PInvoke.HTMAXBUTTON);

                default:
                    handled = false;
                    break;
            }
            return new IntPtr(PInvoke.HTCLIENT);
        }

        private void FocusButton()
        {
            if (_isButtonFocused) return;

            _button.Background = _hoverColor;
            _isButtonFocused = true;
        }

        private void DefocusButton()
        {
            if (!_isButtonFocused) return;

            _button.Background = DefaultButtonBackground;
            _isButtonFocused = false;
        }

        private bool IsOverButton(IntPtr wParam, IntPtr lParam)
        {
            try
            {
                int positionX = lParam.ToInt32() & 0xffff;
                int positionY = lParam.ToInt32() >> 16;

                Rect rect = new Rect(_button.PointToScreen(new Point()),
                    new Size(_button.Width * _dpiScale, _button.Height * _dpiScale));

                if (rect.Contains(new Point(positionX, positionY)))
                    return true;
            }
            catch (OverflowException)
            {
                return true; // or not to true, that is the question
            }

            return false;
        }

        private void RaiseButtonClick()
        {
            if (new ButtonAutomationPeer(_button).GetPattern(PatternInterface.Invoke) is IInvokeProvider invokeProv)
                invokeProv?.Invoke();
        }

        private void SetHoverColor()
        {
            _hoverColor = (SolidColorBrush)Application.Current.Resources["SystemControlHighlightListLowBrush"] ?? new SolidColorBrush(Color.FromArgb(21, 255, 255, 255));
        }
    }
}
