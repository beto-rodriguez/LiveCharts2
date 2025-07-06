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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WinForms;

/// <inheritdoc cref="IChartView" />
public abstract partial class ChartControl : UserControl, IChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    /// <exception cref="MotionCanvas"></exception>
    protected ChartControl()
    {
        var motionCanvas = new MotionCanvas();
        SuspendLayout();
        motionCanvas.Dock = DockStyle.Fill;
        motionCanvas.Location = new Point(0, 0);
        motionCanvas.Name = "motionCanvas";
        motionCanvas.Size = new Size(150, 150);
        motionCanvas.TabIndex = 0;
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(motionCanvas);
        Name = "CartesianChart";
        ResumeLayout(true);

        LiveCharts.Configure(config => config.UseDefaults());

        CoreChart = CreateCoreChart();

        Observe = new ChartObserver(() => CoreChart?.Update())
            .Collection(nameof(Series))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title));

        motionCanvas.Resize += (s, e) =>
            CoreChart.Update();

        CoreChart.Measuring += OnCoreMeasuring;
        CoreChart.UpdateStarted += OnCoreUpdateStarted;
        CoreChart.UpdateFinished += OnCoreUpdateFinished;

        var c = GetDrawnControl();
        c.MouseDown += OnMouseDown;
        c.MouseMove += OnMouseMove;
        c.MouseUp += OnMouseUp;
        c.MouseLeave += OnMouseLeave;
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Controls[0];

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public Chart CoreChart { get; }

    /// <summary>
    /// Gets the chart observer.
    /// </summary>
    protected ChartObserver Observe { get; }

    bool IChartView.DesignerMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    bool IChartView.IsDarkMode => false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    LvcColor IChartView.BackColor
    {
        get => new(BackColor.R, BackColor.G, BackColor.B, BackColor.A);
        set => BackColor = Color.FromArgb(value.A, value.R, value.G, value.B);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    LvcSize IChartView.ControlSize => new() { Width = Width, Height = Height };

    /// <inheritdoc cref="IChartView.Tooltip" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IChartTooltip? Tooltip { get; set; }

    /// <inheritdoc cref="IChartView.Legend" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IChartLegend? Legend { get => field; set { field = value; CoreChart.Update(); } }

    /// <inheritdoc cref="IChartView.ChartTheme" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Theme? ChartTheme { get; set { field = value; CoreChart?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

    /// <summary>
    /// Gets the drawn control.
    /// </summary>
    /// <returns></returns>
    public Control GetDrawnControl() => Controls[0].Controls[0];

    void IChartView.InvokeOnUIThread(Action action)
    {
        if (!IsHandleCreated) return;
        _ = BeginInvoke(action);
    }

    /// <inheritdoc cref="ContainerControl.OnParentChanged(EventArgs)"/>
    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        CoreChart?.Load();
    }

    /// <summary>
    /// Raises the <see cref="E:HandleDestroyed" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <returns></returns>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        Observe.Dispose();
        CoreChart?.Unload();
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

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (ModifierKeys > 0) return;
        CoreChart?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y), e.Button == MouseButtons.Right);
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        base.OnMouseUp(e);
        CoreChart?.InvokePointerUp(new LvcPoint(e.Location.X, e.Location.Y), e.Button == MouseButtons.Right);
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        base.OnMouseLeave(e);
        CoreChart?.InvokePointerLeft();
    }

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
