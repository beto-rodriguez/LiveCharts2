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

#if !HAS_OS_LVC && UNO_LVC

// reachable on uno skia renderer
// HAS_OS_LVC is true when the target framework contains any of the following:
// -windows, -android, -ios, -maccatalyst, -tizen
// currently this is the the same file as WinUI, because uno makes this work across platforms
// but by design this file is separated so in the future if there are any uno specific changes

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace LiveChartsCore.Native;

internal partial class PointerController : INativePointerController
{
    private bool _isPinching;
    private DateTime _pressedTime;

    public void InitializeController(object view)
    {
        var winUIView = (UIElement)view;

        winUIView.PointerPressed += OnUnoSkiaPointerPressed;
        winUIView.PointerMoved += OnUnoSkiaPointerMoved;
        winUIView.PointerReleased += OnUnoSkiaPointerReleased;
        winUIView.PointerWheelChanged += OnUnoSkiaPointerWheelChanged;
        winUIView.PointerExited += OnUnoSkiaPointerExited;

        winUIView.ManipulationMode = ManipulationModes.Scale;
        winUIView.ManipulationStarted += OnPinchSarted;
        winUIView.ManipulationDelta += OnPinching;
        winUIView.ManipulationCompleted += OnPinchCompleted;
    }

    public void DisposeController(object view)
    {
        var winUIView = (UIElement)view;

        winUIView.PointerPressed -= OnUnoSkiaPointerPressed;
        winUIView.PointerMoved -= OnUnoSkiaPointerMoved;
        winUIView.PointerReleased -= OnUnoSkiaPointerReleased;
        winUIView.PointerWheelChanged -= OnUnoSkiaPointerWheelChanged;
        winUIView.PointerExited -= OnUnoSkiaPointerExited;

        winUIView.ManipulationStarted -= OnPinchSarted;
        winUIView.ManipulationDelta -= OnPinching;
        winUIView.ManipulationCompleted -= OnPinchCompleted;
    }

    private void OnUnoSkiaPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var element = (UIElement)sender;

        var p = e.GetCurrentPoint(element);
        if (p is null) return;

#if UnoSkia || DESKTOP
        _ = element.CapturePointer(e.Pointer);
#endif

        Pressed?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));

        _pressedTime = DateTime.Now;
    }

    private void OnUnoSkiaPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        // wait 100ms to ensure it is not a pinch gesture.
        if (_isPinching || ((DateTime.Now - _pressedTime).TotalMilliseconds < 100)) return;

        var p = e.GetCurrentPoint(sender as UIElement);
        if (p is null) return;

        Moved?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), e));
    }

    private void OnUnoSkiaPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var element = (UIElement)sender;

        var p = e.GetCurrentPoint(element);
        if (p is null) return;

#if UnoSkia || DESKTOP
        element.ReleasePointerCapture(element.PointerCaptures[0]);
#endif

        Released?.Invoke(
            sender,
            new(new(p.Position.X, p.Position.Y), p.Properties.IsRightButtonPressed, e));
    }

    private void OnUnoSkiaPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(sender as UIElement);
        Scrolled?.Invoke(sender, new(new(p.Position.X, p.Position.Y), p.Properties.MouseWheelDelta, e));
    }

    private void OnUnoSkiaPointerExited(object sender, PointerRoutedEventArgs e) =>
        Exited?.Invoke(sender, new(e));

    private void OnPinchSarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (!NativeHelpers.IsTouchDevice()) return;

        _isPinching = true;
    }

    private void OnPinching(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        var element = (UIElement)sender;

        Pinched?.Invoke(sender, new(e.Delta.Scale, new(e.Position.X, e.Position.Y), e));
    }

    private void OnPinchCompleted(object sender, ManipulationCompletedRoutedEventArgs e) =>
        _isPinching = false;

}

#endif
