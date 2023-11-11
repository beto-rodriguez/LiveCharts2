// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Maui.Devices;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// The chart behaviour for MAUI.
/// </summary>
public class ChartBehaviour : Behaviours.ChartBehaviour
{
    /// <summary>
    /// Attaches the native events on the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void On(Microsoft.Maui.Controls.VisualElement element)
    {
        element.HandlerChanged += (sender, e) =>
        {
            ScreenSize = new(
                (float)DeviceDisplay.MainDisplayInfo.Width,
                (float)DeviceDisplay.MainDisplayInfo.Height);
            Density = DeviceDisplay.MainDisplayInfo.Density;

#if ANDROID

            var contentViewGroup = (Microsoft.Maui.Platform.ContentViewGroup?)element.Handler?.PlatformView
                ?? throw new System.Exception("Unable to cast to ContentViewGroup");

            contentViewGroup.Touch += OnAndroidTouched;
            contentViewGroup.Hover += OnAndroidHover;

#endif

#if MACCATALYST || IOS

            var contentView = (Microsoft.Maui.Platform.ContentView?)element.Handler?.PlatformView
                ?? throw new System.Exception("Unable to cast to ContentView");

            contentView.UserInteractionEnabled = true;

            contentView.AddGestureRecognizer(GetMacCatalystHover(contentView));
            contentView.AddGestureRecognizer(GetMacCatalystLongPress(contentView));
            contentView.AddGestureRecognizer(GetMacCatalystPinch(contentView));
            contentView.AddGestureRecognizer(GetMacCatalystOnPan(contentView));

#endif

#if WINDOWS

            var contentPanel = (Microsoft.UI.Xaml.UIElement?)element.Handler?.PlatformView
                ?? throw new System.Exception("Unable to cast to ContentPanel");

            contentPanel.PointerPressed += OnWindowsPointerPressed;
            contentPanel.PointerMoved += OnWindowsPointerMoved;
            contentPanel.PointerReleased += OnWindowsPointerReleased;
            contentPanel.PointerWheelChanged += OnWindowsPointerWheelChanged;
            contentPanel.PointerExited += OnWindowsPointerExited;

#endif

        };
    }
}
