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

#if WINDOWS

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace LiveChartsCore.Behaviours;

/// <summary>
/// A class that adds platform-specific events to the chart.
/// </summary>
public partial class ChartBehaviour
{
    protected void OnWindowsPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        Pressed?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));
    }

    protected void OnWindowsPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        Moved?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), e));
    }

    protected void OnWindowsPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        Released?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));
    }

    protected void OnWindowsPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        Scrolled?.Invoke(sender, new(new(p.Position.X, p.Position.Y), p.Properties.MouseWheelDelta, e));
    }

    protected void OnWindowsPointerExited(object sender, PointerRoutedEventArgs e)
    {
        Exited?.Invoke(sender, new(e));
    }
}

#endif
