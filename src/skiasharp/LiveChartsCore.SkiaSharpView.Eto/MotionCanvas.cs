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
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using Eto.SkiaDraw;
using LiveChartsCore.Motion;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <summary>
/// The motion canvas control for windows forms, <see cref="MotionCanvas{TDrawingContext}"/>.
/// </summary>
public class MotionCanvas : SkiaDrawable
{
    private bool _isDrawingLoopRunning = false;
    private List<PaintSchedule<SkiaSharpDrawingContext>> _paintTasksSchedule = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        Paint += new EventHandler<SKPaintEventArgs>(SkControl_PaintSurface);

        CanvasCore.Invalidated += CanvasCore_Invalidated;
    }

    /// <summary>
    /// Gets or sets the paint tasks.
    /// </summary>
    /// <value>
    /// The paint tasks.
    /// </value>
    public List<PaintSchedule<SkiaSharpDrawingContext>> PaintTasks
    {
        get => _paintTasksSchedule;
        set
        {
            _paintTasksSchedule = value;
            OnPaintTasksChanged();
        }
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

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        base.OnUnLoad(e);

        CanvasCore.Invalidated -= CanvasCore_Invalidated;
        CanvasCore.Dispose();
    }

    private void SkControl_PaintSurface(object sender, SKPaintEventArgs e)
    {
        CanvasCore.DrawFrame(new SkiaSharpDrawingContext(CanvasCore, e.Info, e.Surface, e.Surface.Canvas));
    }

    private void CanvasCore_Invalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
    {
        RunDrawingLoop();
    }

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / MaxFps);
        while (!CanvasCore.IsValid)
        {
            Invalidate();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private void OnPaintTasksChanged()
    {
        var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>>();

        foreach (var item in _paintTasksSchedule)
        {
            item.PaintTask.SetGeometries(CanvasCore, item.Geometries);
            _ = tasks.Add(item.PaintTask);
        }

        CanvasCore.SetPaintTasks(tasks);
    }
}
