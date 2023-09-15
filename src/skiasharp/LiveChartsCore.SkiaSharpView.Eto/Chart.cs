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
using Eto.Forms;
using Eto.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.VisualElements;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.SkiaSharpView.SKCharts;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartView" />
public abstract class Chart : Panel, IChartView<SkiaSharpDrawingContext>
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
    private CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;
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

        motionCanvas = new MotionCanvas { MaxFps = 65 };
        motionCanvas.SizeChanged += OnResized;

        UpdateLegendLayout();

        BackgroundColor = Colors.White;

        LiveCharts.Configure(config => config.UseDefaults());

        InitializeCore();

        if (core is null) throw new Exception("Core not found!");
        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;

        var c = motionCanvas;
        c.MouseMove += OnMouseMove;
        c.MouseLeave += Chart_MouseLeave;

        Load += Chart_Load;

        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
    }

    private void UpdateLegendLayout()
    {
        var layout = new DynamicLayout();

        if (this.legend is Control legend)
        {
            if (LegendPosition is LegendPosition.Left or LegendPosition.Right)
                _ = layout.BeginHorizontal();
            else
                _ = layout.BeginVertical();

            if (LegendPosition is LegendPosition.Top)
                layout.AddCentered(legend, horizontalCenter: true);
            if (LegendPosition is LegendPosition.Left)
                layout.AddCentered(legend, verticalCenter: true);

            _ = layout.Add(motionCanvas, xscale: true, yscale: true);

            if (LegendPosition is LegendPosition.Bottom)
                layout.AddCentered(legend, horizontalCenter: true);
            if (LegendPosition is LegendPosition.Right)
                layout.AddCentered(legend, verticalCenter: true);
        }
        else
        {
            _ = layout.Add(motionCanvas, xscale: true, yscale: true);
        }

        Content = layout;
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
    bool IChartView.DesignerMode => false;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => new((byte)BackgroundColor.Rb, (byte)BackgroundColor.Gb, (byte)BackgroundColor.Bb, (byte)BackgroundColor.Ab);
        set => BackgroundColor = Color.FromArgb(value.R, value.G, value.B, value.A);
    }

    LvcSize IChartView.ControlSize =>
        // return the full control size as a workaround when the legend is not set.
        // for some reason Eto.Forms has not loaded the correct size at this point when the control loads.
        LegendPosition == LegendPosition.Hidden
            ? new LvcSize() { Width = ClientSize.Width, Height = ClientSize.Height }
            : new LvcSize() { Width = motionCanvas.Width, Height = motionCanvas.Height };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext { get => CoreCanvas.Sync; set { CoreCanvas.Sync = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.DefaultSettings.AnimationsSpeed;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public Func<float, float>? EasingFunction { get; set; } = LiveCharts.DefaultSettings.EasingFunction;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; UpdateLegendLayout(); OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendTextPaint { get => _legendTextPaint; set { _legendTextPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendBackgroundPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendBackgroundPaint { get => _legendBackgroundPaint; set { _legendBackgroundPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextSize" />
    public double? LegendTextSize { get => _legendTextSize; set { _legendTextSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get => legend; set { legend = value; UpdateLegendLayout(); OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get => tooltip; set { tooltip = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextPaint" />
    public IPaint<SkiaSharpDrawingContext>? TooltipTextPaint { get => _tooltipTextPaint; set { _tooltipTextPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipBackgroundPaint" />
    public IPaint<SkiaSharpDrawingContext>? TooltipBackgroundPaint { get => _tooltipBackgroundPaint; set { _tooltipBackgroundPaint = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextSize" />
    public double? TooltipTextSize { get => _tooltipTextSize; set { _tooltipTextSize = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
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

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title"/>
    public VisualElement<SkiaSharpDrawingContext>? Title { get => _title; set { _title = value; OnPropertyChanged(); } }

    #endregion

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public abstract IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic);

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public abstract IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point);

    internal Point GetCanvasPosition()
    {
        return motionCanvas.Location;
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        _ = Application.Instance.InvokeAsync(action);
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
        core?.Update();
    }

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        if (tooltip is IDisposable disposableTooltip)
        {
            // disposableTooltip.Dispose();

            // tooltip = null;
        }

        base.OnUnLoad(e);

        _visualsObserver = null!;
        core?.Unload();
        OnUnloading();
    }

    /// <summary>
    /// Called when the control is being unloaded.
    /// </summary>
    protected virtual void OnUnloading() { }

    /// <inheritdoc cref="Control.OnLoadComplete(EventArgs)"/>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
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

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged();
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
