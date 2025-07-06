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
using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartView" />
public abstract partial class ChartControl : Panel, IChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    protected ChartControl()
    {
        var motionCanvas = new MotionCanvas();

        Content = motionCanvas;
        BackgroundColor = Colors.White;

        LiveCharts.Configure(config => config.UseDefaults());

        CoreChart = CreateCoreChart();

        Observe = new ChartObserver(() => CoreChart?.Update())
            .Collection(nameof(Series))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title));

        Content.SizeChanged += (s, e) =>
            CoreChart.Update();

        CoreChart.Measuring += OnCoreMeasuring;
        CoreChart.UpdateStarted += OnCoreUpdateStarted;
        CoreChart.UpdateFinished += OnCoreUpdateFinished;

        Content.MouseDown += OnMouseDown;
        Content.MouseMove += OnMouseMove;
        Content.MouseUp += OnMouseUp;
        Content.MouseLeave += OnMouseLeave;
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content;

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public Chart CoreChart { get; }

    /// <summary>
    /// Gets the chart observer.
    /// </summary>
    protected ChartObserver Observe { get; }

    bool IChartView.DesignerMode => false;

    bool IChartView.IsDarkMode => false;

    LvcColor IChartView.BackColor
    {
        get => new((byte)BackgroundColor.Rb, (byte)BackgroundColor.Gb, (byte)BackgroundColor.Bb, (byte)BackgroundColor.Ab);
        set => BackgroundColor = Color.FromArgb(value.R, value.G, value.B, value.A);
    }

    LvcSize IChartView.ControlSize => new() { Width = Content.Width, Height = Content.Height };

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get; set; }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => field; set { field = value; CoreChart.Update(); } }

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get; set { field = value; CoreChart?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => CanvasView.CanvasCore;

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

    void IChartView.InvokeOnUIThread(Action action) =>
        _ = Application.Instance.InvokeAsync(action);

    /// <inheritdoc cref="Control.OnLoadComplete(EventArgs)"/>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
        CoreChart.Load();
    }

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        base.OnUnLoad(e);
        Observe.Dispose();
        CoreChart.Unload();
    }

    /// <summary>
    /// Creates the core chart instance for rendering and manipulation.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by derived classes to provide     a specific
    /// chart type. The returned <see cref="Chart"/> object represents the     foundational chart structure, which can
    /// be further customized or populated     with data.</remarks>
    /// <returns>A <see cref="Chart"/> object that serves as the base chart instance.</returns>
    protected abstract Chart CreateCoreChart();

    private void OnCoreUpdateFinished(IChartView chart) =>
        UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart) =>
        UpdateStarted?.Invoke(this);

    private void OnCoreMeasuring(IChartView chart) =>
        Measuring?.Invoke(this);

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        base.OnMouseMove(e);
        CoreChart?.InvokePointerMove(new LvcPoint(e.Location.X, e.Location.Y));
    }

    private void OnMouseDown(object? sender, MouseEventArgs e) =>
        //if (ModifierKeys > 0) return; // is this supported in Eto.Forms?
        CoreChart?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y), e.Buttons != MouseButtons.Primary);

    private void OnMouseUp(object? sender, MouseEventArgs e) =>
        CoreChart?.InvokePointerUp(new LvcPoint(e.Location.X, e.Location.Y), e.Buttons != MouseButtons.Primary);

    private void OnMouseLeave(object? sender, MouseEventArgs e) =>
        CoreChart?.InvokePointerLeft();

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(
        LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
            => CoreChart.GetPointsAt(point, strategy, findPointFor);

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
        => CoreChart.GetVisualsAt(point);

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints) =>
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer) =>
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs(CoreChart, visualElements, pointer));

    void IChartView.Invalidate() =>
        CoreCanvas.Invalidate();
}
