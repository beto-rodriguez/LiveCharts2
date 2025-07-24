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
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// The motion canvas control for avalonia, <see cref="CoreMotionCanvas"/>.
/// </summary>
public class MotionCanvas : UserControl
{
    private bool _isDeatached = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        CoreMotionCanvas.s_externalRenderer = "Avalonia";
        AttachedToVisualTree += OnAttached;
        DetachedFromVisualTree += OnDetached;
    }

    internal IAvaloniaList<ILogical> Children => LogicalChildren;

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public CoreMotionCanvas CanvasCore { get; } = new();

    /// <summary>
    /// Renders the control.
    /// </summary>p
    /// <param name="context"></param>
    public override void Render(DrawingContext context)
    {
        if (_isDeatached) return;

        context.Custom(new ChartFrameOperation(
            CanvasCore, new Rect(0, 0, Bounds.Width, Bounds.Height), GetBackground().AsSKColor()));

        if (CanvasCore.IsValid) return;
        _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    private void OnCanvasCoreInvalidated(CoreMotionCanvas sender) =>
        InvalidateVisual();

    private void OnAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _isDeatached = false;
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;
    }

    private void OnDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _isDeatached = true;
        CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
        CanvasCore.Dispose();
    }

    // based on:
    // https://github.com/AvaloniaUI/Avalonia/blob/release/11.0.0/samples/RenderDemo/Pages/CustomSkiaPage.cs
    private class ChartFrameOperation(
        CoreMotionCanvas motionCanvas,
        Rect bounds,
        SKColor background)
            : ICustomDrawOperation
    {
        public Rect Bounds { get; } = bounds;

        public void Render(ImmediateDrawingContext context)
        {
            if (!context.TryGetFeature<ISkiaSharpApiLeaseFeature>(out var leaseFeature))
                throw new Exception("SkiaSharp is not supported.");

            using var lease = leaseFeature.Lease();
            if (lease.SkSurface is null) return;

            motionCanvas.DrawFrame(
                new SkiaSharpDrawingContext(motionCanvas,
                    new SKImageInfo((int)Bounds.Width, (int)Bounds.Height),
                    lease.SkSurface,
                    background,
                    false));
        }

        public void Dispose() { }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation? other) => false;
    }

    private LiveChartsCore.Drawing.LvcColor GetBackground()
    {
        var parentBg = Parent is UserControl control && control.Background is SolidColorBrush bg
            ? new LiveChartsCore.Drawing.LvcColor(bg.Color.R, bg.Color.G, bg.Color.B, bg.Color.A)
            : LiveChartsCore.Drawing.LvcColor.Empty;

        return parentBg != LiveChartsCore.Drawing.LvcColor.Empty
            ? parentBg
            : CanvasCore._virtualBackgroundColor;
    }
}
