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
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;
using a = Avalonia.Media;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// The motion canvas control for avalonia, <see cref="MotionCanvas{TDrawingContext}"/>.
/// </summary>
public class MotionCanvas : UserControl
{
    private bool _isDeatached = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeComponent();
        AttachedToVisualTree += MotionCanvas_AttachedToVisualTree;
        DetachedFromVisualTree += MotionCanvas_DetachedFromVisualTree;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// The paint tasks property
    /// </summary>
    public static readonly AvaloniaProperty<List<PaintSchedule<SkiaSharpDrawingContext>>> PaintTasksProperty =
        AvaloniaProperty.Register<MotionCanvas, List<PaintSchedule<SkiaSharpDrawingContext>>>(nameof(PaintTasks), inherits: true);

    /// <summary>
    /// Gets or sets the paint tasks.
    /// </summary>
    /// <value>
    /// The paint tasks.
    /// </value>
    public List<PaintSchedule<SkiaSharpDrawingContext>> PaintTasks
    {
        get => (List<PaintSchedule<SkiaSharpDrawingContext>>?)GetValue(PaintTasksProperty) ?? throw new Exception($"{nameof(PaintTasks)} must not be null.");
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
    /// Renders the control.
    /// </summary>p
    /// <param name="context"></param>
    public override void Render(a.DrawingContext context)
    {
        if (_isDeatached) return;
        var drawOperation = new CustomDrawOp(this, CanvasCore, new Rect(0, 0, Bounds.Width, Bounds.Height));
        context.Custom(drawOperation);

        if (CanvasCore.IsValid) return;
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    /// <inheritdoc cref="OnPropertyChanged(AvaloniaPropertyChangedEventArgs)"/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(PaintTasks))
        {
            var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>>();

            foreach (var item in PaintTasks)
            {
                item.PaintTask.SetGeometries(CanvasCore, item.Geometries);
                _ = tasks.Add(item.PaintTask);
            }

            CanvasCore.SetPaintTasks(tasks);
        }
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        InvalidateVisual();
    }

    private void MotionCanvas_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        _isDeatached = false;
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
    }

    private void MotionCanvas_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        _isDeatached = true;
        CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
        CanvasCore.Dispose();
    }

    // based on:
    // https://github.com/AvaloniaUI/Avalonia/blob/release/11.0.0-preview1/samples/RenderDemo/Pages/CustomSkiaPage.cs
    private class CustomDrawOp : ICustomDrawOperation
    {
        private readonly MotionCanvas _avaloniaControl;
        private readonly MotionCanvas<SkiaSharpDrawingContext> _motionCanvas;

        public CustomDrawOp(
            MotionCanvas avaloniaControl, MotionCanvas<SkiaSharpDrawingContext> motionCanvas, Rect bounds)
        {
            _avaloniaControl = avaloniaControl;
            _motionCanvas = motionCanvas;
            Bounds = bounds;
        }

        public void Dispose() { }

        public Rect Bounds { get; }

        public bool HitTest(Point p)
        {
            return false;
        }

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public void Render(a.ImmediateDrawingContext context)
        {
            if (!context.TryGetFeature<ISkiaSharpApiLeaseFeature>(out var leaseFeature))
                throw new Exception("SkiaSharp is not supported.");

            using var lease = leaseFeature.Lease();

#if DEBUG
            if (LiveCharts.EnableLogging)
            {
                Trace.WriteLine(
                $"[rendering] ".PadRight(60) +
                $"tread: {Environment.CurrentManagedThreadId}");
            }
#endif
            _motionCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    _motionCanvas,
                    new SKImageInfo((int)Bounds.Width, (int)Bounds.Height),
                    lease.SkSurface,
                    lease.SkCanvas,
                    false));
        }
    }
}
