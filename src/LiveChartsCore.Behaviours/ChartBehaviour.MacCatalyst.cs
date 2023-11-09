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

#if MACCATALYST || IOS

using System;
using CoreGraphics;
using LiveChartsCore.Drawing;
using UIKit;

namespace LiveChartsCore.Behaviours;

/// <summary>
/// A class that adds platform-specific events to the chart.
/// </summary>
public partial class ChartBehaviour
{
    private DateTime _previousPress = DateTime.MinValue;

    protected UIHoverGestureRecognizer GetMacCatalystHover(UIView view)
    {
        return new UIHoverGestureRecognizer((UIHoverGestureRecognizer e) =>
        {
            switch (e.State)
            {
                case UIGestureRecognizerState.Changed:
                    var p = e.LocationInView(view);
                    Moved?.Invoke(view, new(new(p.X, p.Y), e));
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                case UIGestureRecognizerState.Ended:
                    Exited?.Invoke(view, new(e));
                    break;
                case UIGestureRecognizerState.Possible:
                case UIGestureRecognizerState.Began:
                default:
                    break;
            }
        });
    }

    protected UILongPressGestureRecognizer GetMacCatalystLongPress(UIView view)
    {
        return new UILongPressGestureRecognizer((UILongPressGestureRecognizer e) =>
        {
            var location = e.LocationInView(view);
            var p = new LvcPoint((float)location.X, (float)location.Y);
            var isRightClick = (DateTime.Now - _previousPress).TotalMilliseconds < 500;
            var isPinch = e.NumberOfTouches > 1;

            switch (e.State)
            {
                case UIGestureRecognizerState.Began:
                    Pressed?.Invoke(view, new(p, isRightClick, e));
                    _previousPress = DateTime.Now;
                    break;
                case UIGestureRecognizerState.Changed:
                    Moved?.Invoke(view, new(p, e));
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Ended:
                    Released?.Invoke(view, new(p, isRightClick, e));
                    break;
                case UIGestureRecognizerState.Possible:
                case UIGestureRecognizerState.Failed:
                default:
                    break;
            }
        })
        {
            MinimumPressDuration = 0,
            ShouldRecognizeSimultaneously = (g1, g2) => true
        };
    }

    private float _previousScale = 1;
    protected UIPinchGestureRecognizer GetMacCatalystPinch(UIView view)
    {
        return new UIPinchGestureRecognizer((UIPinchGestureRecognizer e) =>
        {
            var p = e.LocationInView(view);

            switch (e.State)
            {
                case UIGestureRecognizerState.Began:
                    _previousScale = 1;
                    break;
                case UIGestureRecognizerState.Changed:
                    var s = (float)e.Scale;
                    var delta = _previousScale - s;
                    Pinched?.Invoke(view, new(1 - delta, new(p.X, p.Y), e));
                    _previousScale = s;
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                    break;
                case UIGestureRecognizerState.Possible:
                case UIGestureRecognizerState.Failed:
                default:
                    break;
            }
        })
        {
            ShouldRecognizeSimultaneously = (g1, g2) => true
        };
    }

    private CGPoint? _last;
    protected UIPanGestureRecognizer GetMacCatalystOnPan(UIView view)
    {
        return new UIPanGestureRecognizer((UIPanGestureRecognizer e) =>
        {
            var l = e.LocationInView(view);
            _last ??= l;
            var delta = _last.Value.Y - l.Y;
            var isZoom = e.NumberOfTouches == 0;
            var tolerance = 5; // just a factor to avoid multiple calls.

            if (e.State == UIGestureRecognizerState.Ended || !isZoom || Math.Abs(delta) < tolerance) return;
            Scrolled?.Invoke(view, new(new(l.X, l.Y), delta, e));
            _last = l;
        })
        {
            AllowedScrollTypesMask = UIScrollTypeMask.Discrete | UIScrollTypeMask.Continuous,
            MinimumNumberOfTouches = 0,
            ShouldRecognizeSimultaneously = (g1, g2) => true
        };
    }
}

#endif
