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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms;

/// <summary>
/// The motion canvas control fro Xamarin, <see cref="MotionCanvas{TDrawingContext}"/>.
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MotionCanvas : ContentView
{
    private bool _isDrawingLoopRunning = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeComponent();
        if (skiaElement is null)
            throw new Exception(
                $"SkiaElement not found. This was probably caused because the control {nameof(MotionCanvas)} template was overridden, " +
                $"If you override the template please add an {nameof(SKCanvasView)} to the template and name it 'skiaElement'");

        skiaElement.PaintSurface += OnCanvasViewPaintSurface;
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
    }

    /// <summary>
    /// The paint tasks property
    /// </summary>
    public static readonly BindableProperty PaintTasksProperty = BindableProperty.Create(
        nameof(PaintTasks), typeof(List<PaintSchedule<SkiaSharpDrawingContext>>),
        typeof(MotionCanvas), propertyChanged: PaintTasksChanged);

    /// <summary>
    /// Gets the sk canvas view.
    /// </summary>
    /// <value>
    /// The sk canvas view.
    /// </value>
    public SKCanvasView SkCanvasView => skiaElement;

    /// <summary>
    /// Gets or sets the frames per second.
    /// </summary>
    /// <value>
    /// The frames per second.
    /// </value>
    public double FramesPerSecond { get; set; } = 60;

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
    public void Invalidate()
    {
        MainThread.BeginInvokeOnMainThread(RunDrawingLoop);
    }

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

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        var scale = DeviceDisplay.MainDisplayInfo.Density;
        args.Surface.Canvas.Scale((float)scale, (float)scale);

        CanvasCore.DrawFrame(new SkiaSharpDrawingContext(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        Invalidate();
    }

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / FramesPerSecond);
        while (!CanvasCore.IsValid)
        {
            skiaElement.InvalidateSurface();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private static void PaintTasksChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var motionCanvas = (MotionCanvas)bindable;

        var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>>();

        foreach (var item in motionCanvas.PaintTasks)
        {
            item.PaintTask.SetGeometries(motionCanvas.CanvasCore, item.Geometries);
            _ = tasks.Add(item.PaintTask);
        }

        motionCanvas.CanvasCore.SetPaintTasks(tasks);
    }
}
