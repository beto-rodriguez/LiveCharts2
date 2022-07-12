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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LiveChartsCore.SkiaSharpView.Uno.WinUI.Helpers;
using SkiaSharp.Views.Windows;
using Windows.Graphics.Display;

#if __ANDROID__
using Android.Views;
#endif

namespace LiveChartsCore.SkiaSharpView.Uno.WinUI;

/// <inheritdoc cref="MotionCanvas{TDrawingContext}"/>
public sealed partial class MotionCanvas : UserControl
{
    private readonly SKXamlCanvas _skiaElement;
    private bool _isDrawingLoopRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        var canvas = (SKXamlCanvas)FindName("canvas");
        _skiaElement = canvas;
        _skiaElement.PaintSurface += OnPaintSurface;

#if __ANDROID__
        _scaleDetector = new ScaleGestureDetector(Context, new AndroidScaleListener(this));
#endif
    }

    #region properties

    /// <summary>
    /// The paint tasks property
    /// </summary>
    public static readonly DependencyProperty PaintTasksProperty =
        DependencyProperty.Register(
            nameof(PaintTasks), typeof(List<PaintSchedule<SkiaSharpDrawingContext>>), typeof(MotionCanvas),
            new PropertyMetadata(new List<PaintSchedule<SkiaSharpDrawingContext>>(), new PropertyChangedCallback(OnPaintTaskChanged)));

    /// <summary>
    /// Gets or sets the paint tasks.
    /// </summary>
    /// <value>
    /// The paint tasks.
    /// </value>
    public List<PaintSchedule<SkiaSharpDrawingContext>> PaintTasks
    {
        get => (List<PaintSchedule<SkiaSharpDrawingContext>>)GetValue(PaintTasksProperty);
        set => SetValue(PaintTasksProperty, value);
    }

    /// <summary>
    /// Gets or sets the frames per second.
    /// </summary>
    /// <value>
    /// The frames per second.
    /// </value>
    public double MaxFps { get; set; } = 35;

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

    #endregion

    /// <summary>
    /// Called when the canvas detects a pinch gesture.
    /// </summary>
    public event PinchHandler? Pinched;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
        var scaleFactor = XamlRoot.RasterizationScale;
        args.Surface.Canvas.Scale((float)scaleFactor, (float)scaleFactor);
        CanvasCore.DrawFrame(new SkiaSharpDrawingContext(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
    }

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning || _skiaElement == null) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / MaxFps);
        while (!CanvasCore.IsValid)
        {
            _skiaElement.Invalidate();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        RunDrawingLoop();
    }

    private static void OnPaintTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var motionCanvas = (MotionCanvas)sender;

        var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>>();

        foreach (var item in motionCanvas.PaintTasks ?? Enumerable.Empty<PaintSchedule<SkiaSharpDrawingContext>>())
        {
            item.PaintTask.SetGeometries(motionCanvas.CanvasCore, item.Geometries);
            _ = tasks.Add(item.PaintTask);
        }

        motionCanvas.CanvasCore.SetPaintTasks(tasks);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
        CanvasCore.Dispose();
    }

    #region ANDROID
#if __ANDROID__
    // based on:
    //https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/touch/android-touch-walkthrough

    private readonly ScaleGestureDetector _scaleDetector;

    private int _activePointerId = -1;
    private float _lastTouchX;
    private float _lastTouchY;
    private float _posX;
    private float _posY;

    private class AndroidScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        private readonly MotionCanvas _canvas;

        public AndroidScaleListener(MotionCanvas canvas)
        {
            _canvas = canvas;
        }

        /// <summary>
        /// Called on scale gesture.
        /// </summary>
        /// <param name="detector"></param>
        /// <returns></returns>
        public override bool OnScale(ScaleGestureDetector? detector)
        {
            if (detector is null) return false;

            if (detector.ScaleFactor != 1.0f)
            {
                _canvas.InvokePinch(
                    new LiveChartsPinchEventArgs
                    {
                        PinchStart = new LvcPoint { X = _canvas._lastTouchX, Y = _canvas._lastTouchY },
                        Scale = detector.ScaleFactor
                    });
            }
            return true;
        }
    }

    public override bool OnTouchEvent(MotionEvent? ev)
    {
        if (ev is null) return false;

        _ = _scaleDetector.OnTouchEvent(ev);

        var action = ev.Action & MotionEventActions.Mask;
        int pointerIndex;

        switch (action)
        {
            case MotionEventActions.Down:
                _lastTouchX = ev.GetX();
                _lastTouchY = ev.GetY();
                _activePointerId = ev.GetPointerId(0);
                break;

            case MotionEventActions.Move:
                pointerIndex = ev.FindPointerIndex(_activePointerId);
                var x = ev.GetX(pointerIndex);
                var y = ev.GetY(pointerIndex);
                if (!_scaleDetector.IsInProgress)
                {
                    // Only move the ScaleGestureDetector isn't already processing a gesture.
                    var deltaX = x - _lastTouchX;
                    var deltaY = y - _lastTouchY;
                    _posX += deltaX;
                    _posY += deltaY;
                }

                _lastTouchX = x;
                _lastTouchY = y;
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                // We no longer need to keep track of the active pointer.
                _activePointerId = -1;
                break;

            case MotionEventActions.PointerUp:
                // check to make sure that the pointer that went up is for the gesture we're tracking.
                pointerIndex = (int)(ev.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
                var pointerId = ev.GetPointerId(pointerIndex);
                if (pointerId == _activePointerId)
                {
                    // This was our active pointer going up. Choose a new
                    // action pointer and adjust accordingly
                    var newPointerIndex = pointerIndex == 0 ? 1 : 0;
                    _lastTouchX = ev.GetX(newPointerIndex);
                    _lastTouchY = ev.GetY(newPointerIndex);
                    _activePointerId = ev.GetPointerId(newPointerIndex);
                }
                break;

            default:
                break;
        }

        return true;
    }

    private void InvokePinch(LiveChartsPinchEventArgs args)
    {
        Pinched?.Invoke(this, args);
    }
#endif
    #endregion
}
