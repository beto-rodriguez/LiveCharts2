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

using LiveChartsCore.Drawing;
using Microsoft.UI.Xaml;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The chart behaviour for WinUI and Uno Platform.
/// </summary>
public partial class ChartBehaviour : Behaviours.ChartBehaviour
{
    private static double s_density;
    private static LvcSize s_screenSize;

    /// <inheritdoc />
    public override LvcSize ScreenSize => s_screenSize;

    /// <inheritdoc />
    public override double Density => s_density;

    /// <summary>
    /// Attaches the native events on the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void On(FrameworkElement element)
    {
        // TODO: Detect the DPI and screen size changes.

#if HAS_UNO_WINUI
        var currentView = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

        s_density = currentView.LogicalDpi / 96.0f;
        s_screenSize = new(
            currentView.ScreenWidthInRawPixels,
            currentView.ScreenHeightInRawPixels);
#else
        void getDensity(object s, RoutedEventArgs e)
        {
            s_density = element.XamlRoot.RasterizationScale;
            element.Loaded -= getDensity;
        }

        if (element.IsLoaded) getDensity(null!, null!);
        else element.Loaded += getDensity;
#endif

#if ANDROID

        element.Touch += OnAndroidTouched;
        element.Hover += OnAndroidHover;

#elif MACCATALYST || IOS

        element.UserInteractionEnabled = true;
#if MACCATALYST
        element.AddGestureRecognizer(MacCatalystHoverGestureRecognizer);
#endif
        element.AddGestureRecognizer(MacCatalystLongPressGestureRecognizer);
        element.AddGestureRecognizer(MacCatalystPinchGestureRecognizer);
        element.AddGestureRecognizer(MacCatalystPanGestureRecognizer);

#elif WINDOWS

        element.PointerPressed += OnWindowsPointerPressed;
        element.PointerMoved += OnWindowsPointerMoved;
        element.PointerReleased += OnWindowsPointerReleased;
        element.PointerWheelChanged += OnWindowsPointerWheelChanged;
        element.PointerExited += OnWindowsPointerExited;

#elif HAS_UNO || HAS_UNO_WINUI

        element.PointerPressed += OnUnoPointerPressed;
        element.PointerMoved += OnUnoPointerMoved;
        element.PointerReleased += OnUnoPointerReleased;
        element.PointerWheelChanged += OnUnoPointerWheelChanged;
        element.PointerExited += OnUnoPointerExited;

#endif
    }

#if (HAS_UNO || HAS_UNO_WINUI) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
    // is this just wasm?

    /// <summary>
    /// On uno pointer pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        InvokePressed(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));
    }

    /// <summary>
    /// On uno pointer moved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        InvokeMoved(
            sender,
            new(new(p.Position.X, p.Position.Y), e));
    }

    /// <summary>
    /// On uno pointer released.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        InvokeReleased(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));
    }

    /// <summary>
    /// On uno pointer wheel changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);

        InvokeScrolled(sender, new(new(p.Position.X, p.Position.Y), p.Properties.MouseWheelDelta, e));
    }

    /// <summary>
    /// On uno pointer exited.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
        InvokeExited(sender, new(e));
#endif
}
