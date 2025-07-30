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
using System.ComponentModel;
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;

namespace LiveChartsCore.SkiaSharpView.WinForms;

/// <summary>
/// The motion canvas control for windows forms, <see cref="CoreMotionCanvas"/>.
/// </summary>
/// <seealso cref="UserControl" />
public partial class MotionCanvas : UserControl, IRenderMode
{
    private IFrameTicker _ticker = null!;

    /// <summary>
    /// Gets the recommended rendering settings for WinForms.
    /// </summary>
    public static RenderingSettings RecommendedWinFormsRenderingSettings { get; } =
        new()
        {
            // GPU disabled in WinForms by default for 2 reasons:
            //   1. https://github.com/mono/SkiaSharp/issues/3309
            //   2. OpenTK pointer events are sluggish (at least in WPF).
            UseGPU = false,

            // TryUseVSync makes no sense when GPU is false.
            // Also, WinForms does not support VSync (at least not implemented by livecharts).
            TryUseVSync = false,

            // Because GPU is false, this is the target FPS:
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };

    static MotionCanvas()
    {
        LiveCharts.Configure(config => config.UseDefaults(RecommendedWinFormsRenderingSettings));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        InitializeComponent();
    }

    event CoreMotionCanvas.FrameRequestHandler IRenderMode.FrameRequest
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    /// <inheritdoc cref="CoreMotionCanvas"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CoreMotionCanvas CanvasCore { get; } = new();

    /// <inheritdoc cref="Control.CreateHandle()"/>
    protected override void CreateHandle()
    {
        base.CreateHandle();
        _ticker = new AsyncLoopTicker();
        _ticker.InitializeTicker(CanvasCore, this);
    }

    /// <inheritdoc cref="Control.OnHandleDestroyed(EventArgs)"/>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        _ticker.DisposeTicker();
        CanvasCore.Dispose();
    }

    private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e) =>
        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(CanvasCore, e.Info, e.Surface.Canvas, GetBackground().AsSKColor()));

    private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e) =>
        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(CanvasCore, e.Info, e.Surface.Canvas, GetBackground().AsSKColor()));

    private LvcColor GetBackground() =>
        true
            ? new LvcColor(Parent!.BackColor.R, Parent.BackColor.G, Parent.BackColor.B)
            : CanvasCore._virtualBackgroundColor; // are themes relevant in  Win Forms?

    void IRenderMode.InitializeRenderMode(CoreMotionCanvas canvas) =>
        throw new NotImplementedException();

    void IRenderMode.InvalidateRenderer()
    {
        _skControl?.Invalidate();
        _skglControl?.Invalidate();
    }

    void IRenderMode.DisposeRenderMode() =>
        throw new NotImplementedException();
}
