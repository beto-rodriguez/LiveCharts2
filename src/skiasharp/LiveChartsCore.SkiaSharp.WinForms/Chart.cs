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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WinForms;

/// <inheritdoc cref="IChartView" />
public abstract class Chart : UserControl, IChartView<SkiaSharpDrawingContext>
{
    /// <summary>
    /// The core
    /// </summary>
    protected Chart<SkiaSharpDrawingContext>? core;

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend<SkiaSharpDrawingContext>? legend = new SKDefaultLegend();

    /// <summary>
    /// The tool tip
    /// </summary>
    protected IChartTooltip<SkiaSharpDrawingContext>? tooltip = new SKDefaultTooltip();

    /// <summary>
    /// The motion canvas
    /// </summary>
    protected MotionCanvas motionCanvas;

    private LegendPosition _legendPosition = LiveCharts.DefaultSettings.LegendPosition;
    private Margin? _drawMargin = null;
    private TooltipPosition _tooltipPosition = LiveCharts.DefaultSettings.TooltipPosition;
    private VisualElement<SkiaSharpDrawingContext>? _title;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;
    private IEnumerable<ChartElement<SkiaSharpDrawingContext>> _visuals = new List<ChartElement<SkiaSharpDrawingContext>>();
    private IPaint<SkiaSharpDrawingContext>? _legendTextPaint = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.LegendTextPaint;
    private IPaint<SkiaSharpDrawingContext>? _legendBackgroundPaint = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.LegendBackgroundPaint;
    private double? _legendTextSize = LiveCharts.DefaultSettings.LegendTextSize;
    private IPaint<SkiaSharpDrawingContext>? _tooltipTextPaint = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.TooltipTextPaint;
    private IPaint<SkiaSharpDrawingContext>? _tooltipBackgroundPaint = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.TooltipBackgroundPaint;
    private double? _tooltipTextSize = LiveCharts.DefaultSettings.TooltipTextSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    /// <param name="tooltip">The default tool tip control.</param>
    /// <param name="legend">The default legend.</param>
    /// <exception cref="MotionCanvas"></exception>
    protected Chart(IChartTooltip<SkiaSharpDrawingContext>? tooltip, IChartLegend<SkiaSharpDrawingContext>? legend)
    {
        if (tooltip is not null) this.tooltip = tooltip;
        if (legend is not null) this.legend = legend;

        motionCanvas = new MotionCanvas();
        SuspendLayout();
        motionCanvas.Dock = DockStyle.Fill;
        motionCanvas.MaxFps = 65;
        motionCanvas.Location = new Point(0, 0);
        motionCanvas.Name = "motionCanvas";
        motionCanvas.Size = new Size(150, 150);
        motionCanvas.TabIndex = 0;
        motionCanvas.Resize += OnResized;
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(motionCanvas);
        if (this.legend is Control controlLegend)
        {
            var l = controlLegend;
            l.Visible = false;
            l.Dock = DockStyle.Right;
            Controls.Add(l);
        }
        Name = "CartesianChart";
        ResumeLayout(true);

        LiveCharts.Configure(config => config.UseDefaults());

        InitializeCore();

        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        if (core is null) throw new Exception("Core not found!");
        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;

        var c = GetDrawnControl();
        c.MouseMove += OnMouseMove;
        c.MouseLeave += Chart_MouseLeave;

        Load += Chart_Load;
    }

    #region events

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElementsPointerDown"/>
    public event VisualElementsHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    LvcColor IChartView.BackColor
    {
        get => new(BackColor.R, BackColor.G, BackColor.B, BackColor.A);
        set => BackColor = Color.FromArgb(value.A, value.R, value.G, value.B);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    LvcSize IChartView.ControlSize =>
        // return the full control size as a workaround when the legend is not set.
        // for some reason WinForms has not loaded the correct size at this point when the control loads.
        LegendPosition == LegendPosition.Hidden
            ? new LvcSize() { Width = ClientSize.Width, Height = ClientSize.Height }
            : new LvcSize() { Width = motionCanvas.Width, Height = motionCanvas.Height };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

    /// <inheritdoc cref="IChartView.DrawMargin" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VisualElement<SkiaSharpDrawingContext>? Title { get => _title; set { _title = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.SyncContext" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object SyncContext { get => CoreCanvas.Sync; set { CoreCanvas.Sync = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.DefaultSettings.AnimationsSpeed;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<float, float>? EasingFunction { get; set; } = LiveCharts.DefaultSettings.EasingFunction;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextPaint" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPaint<SkiaSharpDrawingContext>? LegendTextPaint { get => _legendTextPaint; set { _legendTextPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendBackgroundPaint" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPaint<SkiaSharpDrawingContext>? LegendBackgroundPaint { get => _legendBackgroundPaint; set { _legendBackgroundPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextSize" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double? LegendTextSize { get => _legendTextSize; set { _legendTextSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get => legend; set => legend = value; }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextPaint" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPaint<SkiaSharpDrawingContext>? TooltipTextPaint { get => _tooltipTextPaint; set { _tooltipTextPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipBackgroundPaint" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPaint<SkiaSharpDrawingContext>? TooltipBackgroundPaint { get => _tooltipBackgroundPaint; set { _tooltipBackgroundPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextSize" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double? TooltipTextSize { get => _tooltipTextSize; set { _tooltipTextSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get => tooltip; set => tooltip = value; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => _visuals;
        set
        {
            _visualsObserver?.Dispose(_visuals);
            _visualsObserver?.Initialize(value);
            _visuals = value;
            OnPropertyChanged();
        }
    }

    #endregion

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public abstract IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic);

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public abstract IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point);

    /// <summary>
    /// Gets the drawn control.
    /// </summary>
    /// <returns></returns>
    public Control GetDrawnControl()
    {
        return Controls[0].Controls[0];
    }

    internal Point GetCanvasPosition()
    {
        return motionCanvas.Location;
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        if (!IsHandleCreated) return;
        _ = BeginInvoke(action);
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected abstract void InitializeCore();

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    /// <returns></returns>
    protected void OnPropertyChanged()
    {
        if (core is null || ((IChartView)this).DesignerMode) return;
        core.Update();
    }

    /// <summary>
    /// Raises the <see cref="E:HandleDestroyed" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <returns></returns>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (tooltip is IDisposable disposableTooltip) disposableTooltip.Dispose();
        base.OnHandleDestroyed(e);

        core?.Unload();
        OnUnloading();
    }

    /// <summary>
    /// Called when the control is being unloaded.
    /// </summary>
    protected virtual void OnUnloading() { }

    /// <inheritdoc cref="ContainerControl.OnParentChanged(EventArgs)"/>
    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        core?.Load();
    }

    private void OnResized(object? sender, EventArgs e)
    {
        if (core is null) return;
        core.Update();
    }

    private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateFinished?.Invoke(this);
    }

    private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
    {
        Measuring?.Invoke(this);
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    /// <summary>
    /// Called when the mouse goes down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnMouseMove(object? sender, MouseEventArgs e)
    {
        core?.InvokePointerMove(new LvcPoint(e.Location.X, e.Location.Y));
    }

    /// <summary>
    /// Called when the mouse leaves the control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Chart_MouseLeave(object? sender, EventArgs e)
    {
        core?.InvokePointerLeft();
    }

    private void Chart_Load(object? sender, EventArgs e)
    {
        core?.Load();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView<SkiaSharpDrawingContext>.OnVisualElementPointerDown(
        IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs<SkiaSharpDrawingContext>(CoreChart, visualElements, pointer));
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
