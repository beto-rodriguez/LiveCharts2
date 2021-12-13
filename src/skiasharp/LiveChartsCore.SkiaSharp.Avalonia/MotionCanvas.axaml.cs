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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;
using a = Avalonia.Media;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// The motion canvas control for avalonia, <see cref="MotionCanvas{TDrawingContext}"/>.
/// </summary>
public class MotionCanvas : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeComponent();
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
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
    /// The back color property
    /// </summary>
    public static readonly AvaloniaProperty<SKColor> BackColorProperty =
        AvaloniaProperty.Register<MotionCanvas, SKColor>(nameof(BackColor), defaultValue: new SKColor(255, 255, 255, 0), inherits: true);

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
    public double FramesPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public SKColor BackColor
    {
        get => (SKColor)GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
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
    /// </summary>
    /// <param name="context"></param>
    public override void Render(a.DrawingContext context)
    {
        var drawOperation = new CustomDrawOp(this, CanvasCore, new Rect(0, 0, Bounds.Width, Bounds.Height), BackColor);
        context.Custom(drawOperation);
    }

    /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})" />
    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
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

        base.OnPropertyChanged(change);
    }

    private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        InvalidateVisual();
    }

    //private void InvalidateOnUIThread()
    //{
    //    _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual);
    //}

    // based on:
    // https://github.com/AvaloniaUI/Avalonia/blob/554aaec5e5cc96c0b4318b6ed1fbf8159f442889/samples/RenderDemo/Pages/CustomSkiaPage.cs
    private class CustomDrawOp : ICustomDrawOperation
    {
        private readonly MotionCanvas _avaloniaControl;
        private readonly MotionCanvas<SkiaSharpDrawingContext> _motionCanvas;
        private readonly SKColor _backColor;

        public CustomDrawOp(
            MotionCanvas avaloniaControl, MotionCanvas<SkiaSharpDrawingContext> motionCanvas, Rect bounds, SKColor backColor)
        {
            _avaloniaControl = avaloniaControl;
            _motionCanvas = motionCanvas;
            _backColor = backColor;
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

        public void Render(IDrawingContextImpl context)
        {
            if (context is not ISkiaDrawingContextImpl skiaContext)
                throw new Exception("SkiaSharp is not supported.");

#if DEBUG
            if (LiveCharts.EnableLogging)
            {
                Trace.WriteLine(
                $"[rendering] ".PadRight(60) +
                $"tread: {Environment.CurrentManagedThreadId}");
            }
#endif

            // why isn't the render method called on the UI thread always?
            // as a workaround we lock the canvas to draw the frame.
            lock (_motionCanvas.Sync)
            {
                _motionCanvas.DrawFrame(
                   new AvaloniaDrawingContext(
                       _motionCanvas, new SKImageInfo((int)Bounds.Width, (int)Bounds.Height), skiaContext.SkSurface, skiaContext.SkCanvas)
                   {
                       BackColor = _backColor
                   });
            }

            if (_motionCanvas.IsValid) return;

            _avaloniaControl.InvalidateVisual();
        }
    }
}
