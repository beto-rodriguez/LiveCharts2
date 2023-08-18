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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <summary>
/// Defines the motion canvas control for WPF, <see cref="MotionCanvas{TDrawingContext}"/>.
/// </summary>
/// <seealso cref="Control" />
public class MotionCanvas : Control
{
    /// <summary>
    /// The skia element
    /// </summary>
    private SKElement? _skiaElement;
    private bool _isDrawingLoopRunning = false;

    static MotionCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MotionCanvas), new FrameworkPropertyMetadata(typeof(MotionCanvas)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

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
    public double MaxFPS { get; set; } = 60;

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

    /// <summary>
    /// Called when the template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _skiaElement = Template.FindName("skiaElement", this) as SKElement;
        if (_skiaElement is null)
            throw new Exception(
                $"SkiaElement not found. This was probably caused because the control {nameof(MotionCanvas)} template was overridden, " +
                $"If you override the template please add an {nameof(SKElement)} to the template and name it 'skiaElement'");

        _skiaElement.PaintSurface += OnPaintSurface;
    }

    /// <inheritdoc cref="OnPaintSurface(object?, SKPaintSurfaceEventArgs)" />
    protected virtual void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
        var density = GetPixelDensity();
        args.Surface.Canvas.Scale(density.dpix, density.dpiy);
        CanvasCore.DrawFrame(new SkiaSharpDrawingContext(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
    }

    private ResolutionHelper GetPixelDensity()
    {
        var presentationSource = PresentationSource.FromVisual(this);
        if (presentationSource is null) return new(1f, 1f);
        var compositionTarget = presentationSource.CompositionTarget;
        if (compositionTarget is null) return new(1f, 1f);

        var matrix = compositionTarget.TransformToDevice;
        return new((float)matrix.M11, (float)matrix.M22);
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        RunDrawingLoop();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
        CanvasCore.Dispose();
    }

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning || _skiaElement is null) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / MaxFPS);
        while (!CanvasCore.IsValid)
        {
            _skiaElement.InvalidateVisual();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private static void OnPaintTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var motionCanvas = (MotionCanvas)sender;

        var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>>();

        foreach (var item in motionCanvas.PaintTasks)
        {
            item.PaintTask.SetGeometries(motionCanvas.CanvasCore, item.Geometries);
            _ = tasks.Add(item.PaintTask);
        }

        motionCanvas.CanvasCore.SetPaintTasks(tasks);
    }
}
