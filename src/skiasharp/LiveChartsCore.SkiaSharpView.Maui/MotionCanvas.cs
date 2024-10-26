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
using System.Threading.Tasks;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// Defines the motion cavnas class for Maui.
/// </summary>
public class MotionCanvas : ContentView
{
    private readonly SKCanvasView _skiaElement;
    private bool _isDrawingLoopRunning = false;
    private bool _isLoaded = true;
    private double _density = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        Content = _skiaElement = new();
        _skiaElement.PaintSurface += OnCanvasViewPaintSurface;

        _density = DeviceDisplay.MainDisplayInfo.Density;
        DeviceDisplay.MainDisplayInfoChanged += MainDisplayInfoChanged;

        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
        Unloaded += OnUnloaded;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Gets or sets the frames per second.
    /// </summary>
    /// <value>
    /// The frames per second.
    /// </value>
    public double MaxFps { get; set; } = 60;

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

    /// <summary>
    /// Invalidates this instance.
    /// </summary>
    /// <returns></returns>
    public void Invalidate() =>
        _ = MainThread.InvokeOnMainThreadAsync(RunDrawingLoop);

    /// <inheritdoc cref="NavigableElement.OnParentSet"/>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent == null)
        {
            CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
            CanvasCore.Dispose();
        }
    }

    private void OnCanvasViewPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
        args.Surface.Canvas.Scale((float)_density, (float)_density);
        CanvasCore.DrawFrame(new(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender) =>
        Invalidate();

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / MaxFps);
        while (!CanvasCore.IsValid && _isLoaded)
        {
            _skiaElement?.InvalidateSurface();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private void OnLoaded(object? sender, EventArgs e) =>
        _isLoaded = true;

    private void OnUnloaded(object? sender, EventArgs e) =>
        _isLoaded = false;

    private void MainDisplayInfoChanged(object? sender, EventArgs e) =>
        _density = DeviceDisplay.MainDisplayInfo.Density;
}
