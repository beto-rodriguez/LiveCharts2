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

#if ANDROID

using Android.Views;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Behaviours;

public abstract partial class ChartBehaviour
{
    private bool _isPinching;
    private bool _isDown;
    private LvcPoint _lastTouch = new();
    private readonly ScaleGestureDetector _scaleDetector = null!;

    protected void OnAndroidHover(object? sender, View.HoverEventArgs e)
    {
        if (e.Event is null) return;

        var p = new LvcPoint(e.Event.GetX() / Density, e.Event.GetY() / Density);
        Moved!.Invoke(sender, new(p, e.Event));
    }

    protected void OnAndroidTouched(object? sender, View.TouchEventArgs e)
    {
        if (e.Event is null) return;

        var p = new LvcPoint(e.Event.GetX() / Density, e.Event.GetY() / Density);
        var isRightClick = false; // can we detect this?
        var isPinch = e.Event.PointerCount > 1;

        _ = _scaleDetector.OnTouchEvent(e.Event);

        switch (e.Event.ActionMasked)
        {
            case MotionEventActions.ButtonPress:
            case MotionEventActions.Pointer1Down:
            case MotionEventActions.Pointer2Down:
            case MotionEventActions.Pointer3Down:
            case MotionEventActions.Down:
                _isPinching = isPinch;
                _isDown = true;
                if (!_isPinching)
                    Pressed?.Invoke(sender, new(p, isRightClick, e.Event));
                break;
            case MotionEventActions.Move:
            case MotionEventActions.HoverMove:
                // the Moved event is only raised when the pointer is down,
                // it is also fired frorm the hover event when the pointer is not down.
                if (!_isPinching && _isDown)
                    Moved?.Invoke(sender, new(p, e.Event));
                break;
            case MotionEventActions.ButtonRelease:
            case MotionEventActions.Pointer1Up:
            case MotionEventActions.Pointer2Up:
            case MotionEventActions.Pointer3Up:
            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                if (!_isPinching && _isDown)
                    Released?.Invoke(sender, new(p, isRightClick, e.Event));
                else
                    _isPinching = false;
                _isDown = false;
                break;
            case MotionEventActions.HoverEnter:
            case MotionEventActions.HoverExit:
            case MotionEventActions.Mask:
            case MotionEventActions.Outside:
            case MotionEventActions.PointerIdMask:
            case MotionEventActions.PointerIdShift:
            default:
                break;
        }

        _lastTouch = p;
    }

    private class CustomScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        private readonly Action<float> _onScaled;

        public CustomScaleListener(Action<float> onSacaled)
        {
            _onScaled = onSacaled;
        }

        public override bool OnScale(ScaleGestureDetector? detector)
        {
            if (detector is null || detector.ScaleFactor == 1) return false;
            _onScaled(detector.ScaleFactor);
            return true;
        }
    }
}

#endif
