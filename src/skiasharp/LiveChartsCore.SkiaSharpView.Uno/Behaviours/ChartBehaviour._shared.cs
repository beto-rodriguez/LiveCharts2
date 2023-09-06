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

using LiveChartsCore.SkiaSharpView.Uno.Behaviours.Events;
using Windows.UI.Xaml;

namespace LiveChartsCore.SkiaSharpView.Uno.Behaviours;

// -----
// NOTE (HELP WANTED?)
// this code is repeated, ideally it should be shared between all the projects
// but I am not able to consume the LiveChartsCore.Behaviours project from here
// the thing is that the #IF WINDOWS is not working as expected, already ttried with
// uap10.0.19041 withhour success.
// -----

/// <summary>
/// A class that adds platform-specific events to the chart.
/// </summary>
public partial class ChartBehaviour
{
    /// <summary>
    /// Gets or sets the screen density.
    /// </summary>
    public double Density { get; set; }

    /// <summary>
    /// Called when the pointer/tap is pressed.
    /// </summary>
    public event PressedHandler? Pressed;

    /// <summary>
    /// Called when the pointer/tap is released.
    /// </summary>
    public event PressedHandler? Released;

    /// <summary>
    /// Called when the pointer/tap moves.
    /// </summary>
    public event ScreenHandler? Moved;

    /// <summary>
    /// Called when the pointer exits the control.
    /// </summary>
    public event Handler? Exited;

    /// <summary>
    /// Called when the control is pinched.
    /// </summary>
    public event PinchHandler? Pinched;

    /// <summary>
    /// Called when the control is scrolled.
    /// </summary>
    public event ScrollHandler? Scrolled;

    /// <summary>
    /// Attaches the native events on the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void On(FrameworkElement element)
    {
#if HAS_UNO_WINUI
        Density = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi / 96.0f;
#else
        Density = element.XamlRoot.RasterizationScale;
#endif

#if ANDROID

        element.Touch += OnAndroidTouched;
        element.Hover += OnAndroidHover;

#endif

#if MACCATALYST || IOS

        element.UserInteractionEnabled = true;
        element.AddGestureRecognizer(GetMacCatalystHover(element));
        element.AddGestureRecognizer(GetMacCatalystLongPress(element));
        element.AddGestureRecognizer(GetMacCatalystPinch(element));
        element.AddGestureRecognizer(GetMacCatalystOnPan(element));

#endif

#if UAP10_0_18362

        element.PointerPressed += OnWindowsPointerPressed;
        element.PointerMoved += OnWindowsPointerMoved;
        element.PointerReleased += OnWindowsPointerReleased;
        element.PointerWheelChanged += OnWindowsPointerWheelChanged;
        element.PointerExited += OnWindowsPointerExited;

#endif
    }
}
