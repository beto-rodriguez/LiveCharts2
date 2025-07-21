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
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// Defines the motion cavnas class for Maui.
/// </summary>
public class MotionCanvas : AbsoluteLayout
{
    private readonly Renderer _renderer;
    private SKCanvasView? _canvasView;
    private SKGLView? _glView;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeView();

        CanvasCore = new();
        _renderer = new(CanvasCore, InvalidateChart);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public CoreMotionCanvas CanvasCore { get; } = new();

    private void InvalidateChart()
    {
        _canvasView?.InvalidateSurface();
        _glView?.InvalidateSurface();
    }

    private void OnCanvasViewPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
        if (_renderer.Density != 1)
            args.Surface.Canvas.Scale(_renderer.Density, _renderer.Density);

        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(
                CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
    }

    private void OnGlViewPaintSurface(object? sender, SKPaintGLSurfaceEventArgs args)
    {
        if (_renderer.Density != 1)
            args.Surface.Canvas.Scale(_renderer.Density, _renderer.Density);

        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(
                CanvasCore, new SKImageInfo((int)Width, (int)Height), args.Surface, args.Surface.Canvas));
    }

    private void InitializeView()
    {
        if (LiveCharts.UseGPU)
        {
            _glView = new SKGLView();
            _glView.PaintSurface += OnGlViewPaintSurface;

            AbsoluteLayout.SetLayoutBounds(_glView, new(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_glView, AbsoluteLayoutFlags.SizeProportional | AbsoluteLayoutFlags.PositionProportional);

            Children.Add(_glView);
        }
        else
        {
            _canvasView = new SKCanvasView();
            _canvasView.PaintSurface += OnCanvasViewPaintSurface;

            AbsoluteLayout.SetLayoutBounds(_canvasView, new(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_canvasView, AbsoluteLayoutFlags.SizeProportional | AbsoluteLayoutFlags.PositionProportional);

            Children.Add(_canvasView);
        }
    }

    private void OnLoaded(object? sender, EventArgs e) =>
        _renderer.Start();

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _renderer.Stop();
        CanvasCore.Dispose();
    }
}

/// <summary>
/// Defines the renderer class for Maui.
/// </summary>
public partial class Renderer
{
    private readonly CoreMotionCanvas _canvas;
    private readonly Action _invalidator;

    /// <summary>
    /// Gets the screen density of the device.
    /// </summary>
    public float Density { get; private set; } = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Renderer"/> class.
    /// </summary>
    /// <param name="canvas">The livecharts canvas.</param>
    /// <param name="invalidator">The action to invalidate the canvas.</param>
    public Renderer(CoreMotionCanvas canvas, Action invalidator)
    {
        Density = (float)DeviceDisplay.MainDisplayInfo.Density;
        DeviceDisplay.MainDisplayInfoChanged += MainDisplayInfoChanged;

        _canvas = canvas;
        _invalidator = invalidator;
    }

    /// <summary>
    /// Starts the rendering loop for the canvas.
    /// </summary>
    public void Start()
    {
        if (!LiveCharts.UseVSync)
        {
            _canvas.Invalidated += StartLiveChartsDrawingLoop;
            return;
        }

#if WINDOWS
        StartWindowsRenderer();
#elif ANDROID
        StartAndroidRenderer();
#else
        // if no platform-specific renderer is available,
        // then use the livecharts drawing loop,
        // slower but works on all platforms

        _canvas.Invalidated += StartLiveChartsDrawingLoop;
#endif
    }

    /// <summary>
    /// Ends the rendering loop for the canvas.
    /// </summary>
    public void Stop()
    {
        if (!LiveCharts.UseVSync)
        {
            _canvas.Invalidated -= StartLiveChartsDrawingLoop;
            return;
        }

#if WINDOWS
        StopWindowsRenderer();
#elif ANDROID
        StopAndroidRenderer();
#else
        _canvas.Invalidated -= StartLiveChartsDrawingLoop;
#endif
    }

    private void StartLiveChartsDrawingLoop(CoreMotionCanvas canvas) =>
        _canvas.RunDrawingLoop(_invalidator);

    private void MainDisplayInfoChanged(object? sender, EventArgs e) =>
        Density = (float)DeviceDisplay.MainDisplayInfo.Density;
}

#if WINDOWS

public partial class Renderer
{
    private void StartWindowsRenderer()
    {
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += OnRendering;
        _canvas.Invalidated += OnLiveChartsCanvasInvalidated;
    }

    private void StopWindowsRenderer()
    {
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= OnRendering;
        _canvas.Invalidated -= OnLiveChartsCanvasInvalidated;
    }

    private void OnRendering(object? sender, object e)
    {
        // this is called on every vsync tick
        if (_canvas.IsValid) return;
        _invalidator();
    }

    private void OnLiveChartsCanvasInvalidated(CoreMotionCanvas obj) =>
        // this is the first call to invalidate the canvas
        // when livecharts detect a change in the data/properties
        _invalidator();
}

#endif

#if ANDROID

public class VSyncTicker(Action onFrameTick)
    : Java.Lang.Object, Android.Views.Choreographer.IFrameCallback
{
    private readonly Android.Views.Choreographer _chor = Android.Views.Choreographer.Instance!;

    public void Start() =>
        _chor.PostFrameCallback(this);

    // frameTimeNanos:
    // the absolute timestamp (in nanoseconds) that the system’s choreographer assigns to this frame.
    public void DoFrame(long frameTimeNanos)
    {
        onFrameTick();
        _chor.PostFrameCallback(this);
    }

    public void Stop() =>
        _chor.RemoveFrameCallback(this);
}

public partial class Renderer
{
    private VSyncTicker _vsyncTicker = null!;

    private void StartAndroidRenderer()
    {
        _vsyncTicker = new VSyncTicker(OnRendering);
        _vsyncTicker.Start();
        _canvas.Invalidated += OnLiveChartsCanvasInvalidated;
    }

    private void StopAndroidRenderer()
    {
        _vsyncTicker.Stop();
        _vsyncTicker = null!;
        _canvas.Invalidated -= OnLiveChartsCanvasInvalidated;
    }

    private void OnRendering()
    {
        // this is called on every vsync tick
        if (_canvas.IsValid) return;
        _invalidator();
    }

    private void OnLiveChartsCanvasInvalidated(CoreMotionCanvas obj) =>
        // this is the first call to invalidate the canvas
        // when livecharts detect a change in the data/properties
        _invalidator();
}

#endif
