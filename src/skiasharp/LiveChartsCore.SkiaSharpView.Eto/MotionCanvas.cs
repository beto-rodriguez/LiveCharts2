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
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Drawing;
using Eto.SkiaDraw;
using LiveChartsCore.Motion;
using LiveChartsCore.Kernel;
using SkiaSharp;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <summary>
/// The motion canvas control for windows forms, <see cref="CoreMotionCanvas"/>.
/// </summary>
public class MotionCanvas : SkiaDrawable, IRenderMode
{
    private IFrameTicker _ticker = null!;

    static MotionCanvas()
    {
        LiveChartsSkiaSharp.EnsureInitialized();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        Paint += new EventHandler<SKPaintEventArgs>(SkControl_PaintSurface);
    }

    event CoreMotionCanvas.FrameRequestHandler IRenderMode.FrameRequest
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public CoreMotionCanvas CanvasCore { get; } = new();

    /// <inheritdoc cref="Control.OnLoadComplete(EventArgs)"/>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
        _ticker = new AsyncLoopTicker();
        _ticker.InitializeTicker(CanvasCore, this);
    }

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        base.OnUnLoad(e);
        _ticker.DisposeTicker();
        CanvasCore.Dispose();
    }

    private void SkControl_PaintSurface(object sender, SKPaintEventArgs e) =>
        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(CanvasCore, e.Surface.Canvas, GetBackground()));

    void IRenderMode.InitializeRenderMode(CoreMotionCanvas canvas) =>
        throw new NotImplementedException();

    void IRenderMode.InvalidateRenderer() =>
        Invalidate();

    void IRenderMode.DisposeRenderMode() =>
        throw new NotImplementedException();

    private SKColor GetBackground() =>
        (Parent as IChartView)?.BackColor.AsSKColor() ?? SKColor.Empty;
}
