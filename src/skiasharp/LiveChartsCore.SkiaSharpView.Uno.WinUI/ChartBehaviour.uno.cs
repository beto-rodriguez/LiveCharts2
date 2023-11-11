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

#if (HAS_UNO || HAS_UNO_WINUI) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS

// Work around for Uno (WASM and SKIA), we use the same code as WinUI, but we
// can not call this from the behaviours assembly because it does not have
// a reference to the Uno assembly. So we have to copy the code here.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The chart behaviour for Uno Platform.
/// </summary>
public partial class ChartBehaviour : Behaviours.ChartBehaviour
{
    /// <summary>
    /// On uno pointer pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerPressed(object sender, PointerRoutedEventArgs e)
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
    protected void OnUnoPointerMoved(object sender, PointerRoutedEventArgs e)
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
    protected void OnUnoPointerReleased(object sender, PointerRoutedEventArgs e)
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
    protected void OnUnoPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);

        InvokeScrolled(sender, new(new(p.Position.X, p.Position.Y), p.Properties.MouseWheelDelta, e));
    }

    /// <summary>
    /// On uno pointer exited.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnoPointerExited(object sender, PointerRoutedEventArgs e)
    {
        InvokeExited(sender, new(e));
    }
}

#endif
