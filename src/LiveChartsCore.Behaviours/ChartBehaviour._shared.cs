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

using LiveChartsCore.Behaviours.Events;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Behaviours;

/// <summary>
/// A class that adds platform-specific events to the chart.
/// </summary>
public abstract partial class ChartBehaviour
{
    /// <summary>
    /// Gets or sets the screen size, only used internally by the Android handler, to implement a
    /// workaround for https://github.com/dotnet/maui/issues/18547.
    /// </summary>
    public LvcSize ScreenSize { get; set; }

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

    internal void InvokePressed(object sender, PressedEventArgs e)
    {
        Pressed?.Invoke(sender, e);
    }

    internal void InvokeReleased(object sender, PressedEventArgs e)
    {
        Released?.Invoke(sender, e);
    }

    internal void InvokeMoved(object sender, ScreenEventArgs e)
    {
        Moved?.Invoke(sender, e);
    }

    internal void InvokeExited(object sender, EventArgs e)
    {
        Exited?.Invoke(sender, e);
    }

    internal void InvokePinched(object sender, PinchEventArgs e)
    {
        Pinched?.Invoke(sender, e);
    }

    internal void InvokeScrolled(object sender, ScrollEventArgs e)
    {
        Scrolled?.Invoke(sender, e);
    }
}
