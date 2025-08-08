﻿// The MIT License(MIT)
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
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore;

/// <summary>
/// Defines a chart,
/// </summary>
public abstract class Chart
{
    #region fields

    internal readonly HashSet<IChartElement> _everMeasuredElements = [];
    internal HashSet<IChartElement> _toDeleteElements = [];
    internal bool _isToolTipOpen = false;
    internal bool _isPointerIn;
    internal LvcPoint _pointerPosition = new(-10, -10);
    internal float _titleHeight = 0f;
    internal LvcSize _legendSize;
    internal bool _preserveFirstDraw = false;
    internal readonly HashSet<int> _drawnSeries = [];
    internal bool _isFirstDraw = true;
    private readonly ActionThrottler _updateThrottler;
    private readonly ActionThrottler _tooltipThrottler;
    private readonly ActionThrottler _panningThrottler;
    private LvcPoint _pointerPanningStartPosition = new(-10, -10);
    private LvcPoint _pointerPanningPosition = new(-10, -10);
    private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
    private bool _isPanning = false;
    private readonly HashSet<ChartPoint> _activePoints = [];
    private LvcSize _previousSize = new();
#if NET5_0_OR_GREATER
    private readonly bool _isMobile;
    private bool _isTooltipCanceled;
#endif

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="defaultPlatformConfig">The default platform configuration.</param>
    /// <param name="view">The chart view.</param>
    /// <param name="kind">The chart kind.</param>
    protected Chart(
        CoreMotionCanvas canvas,
        Action<LiveChartsSettings> defaultPlatformConfig,
        IChartView view,
        ChartKind kind)
    {
        Kind = kind;
        Canvas = canvas;
        canvas.Validated += OnCanvasValidated;
        EasingFunction = EasingFunctions.QuadraticOut;
        LiveCharts.Configure(defaultPlatformConfig);

        _updateThrottler = view.DesignerMode
                ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(100))
                : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
        _updateThrottler.ThrottlerTimeSpan = view.UpdaterThrottler;

        _tooltipThrottler = new ActionThrottler(TooltipThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
        _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));

#if NET5_0_OR_GREATER

        _isMobile = OperatingSystem.IsOSPlatform("Android") || OperatingSystem.IsOSPlatform("iOS");

#endif
    }

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    #region properties

    /// <summary>
    /// Gets the tool tip meta data.
    /// </summary>
    public ToolTipMetaData AutoToolTipsInfo { get; internal set; } = new();

    /// <summary>
    /// Gets the bounds of the chart.
    /// </summary>
    public AnimatableContainer ActualBounds { get; } = new();

    /// <summary>
    /// Gets the measure work.
    /// </summary>
    /// <value>
    /// The measure work.
    /// </value>
    public object MeasureWork { get; protected set; } = new();

    /// <summary>
    /// Gets the kind of the chart.
    /// </summary>
    public ChartKind Kind { get; protected set; }

    /// <summary>
    /// Gets whether the control is loaded.
    /// </summary>
    public bool IsLoaded { get; internal set; } = false;

    /// <summary>
    /// Gets the canvas.
    /// </summary>
    /// <value>
    /// The canvas.
    /// </value>
    public CoreMotionCanvas Canvas { get; private set; }

    /// <summary>
    /// Gets the visible series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public abstract IEnumerable<ISeries> VisibleSeries { get; }

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public abstract IEnumerable<ISeries> Series { get; }

    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public abstract IChartView View { get; }

    /// <summary>
    /// The series context
    /// </summary>
    public SeriesContext SeriesContext { get; protected set; } = new([], null!);

    /// <summary>
    /// Gets the size of the control.
    /// </summary>
    /// <value>
    /// The size of the control.
    /// </value>
    public LvcSize ControlSize { get; protected set; } = new();

    /// <summary>
    /// Gets the draw margin location.
    /// </summary>
    /// <value>
    /// The draw margin location.
    /// </value>
    public LvcPoint DrawMarginLocation { get; protected set; } = new();

    /// <summary>
    /// Gets the size of the draw margin.
    /// </summary>
    /// <value>
    /// The size of the draw margin.
    /// </value>
    public LvcSize DrawMarginSize { get; protected set; } = new();

    /// <summary>
    /// Gets the legend position.
    /// </summary>
    /// <value>
    /// The legend position.
    /// </value>
    public LegendPosition LegendPosition { get; protected set; }

    /// <summary>
    /// Gets the legend.
    /// </summary>
    /// <value>
    /// The legend.
    /// </value>
    public IChartLegend? Legend { get; protected set; }

    /// <summary>
    /// Gets the tooltip position.
    /// </summary>
    /// <value>
    /// The tooltip position.
    /// </value>
    public TooltipPosition TooltipPosition { get; protected set; }

    /// <summary>
    /// Gets the tooltip finding strategy.
    /// </summary>
    /// <value>
    /// The tooltip finding strategy.
    /// </value>
    public FindingStrategy FindingStrategy { get; protected set; }

    /// <summary>
    /// Gets the tooltip.
    /// </summary>
    /// <value>
    /// The tooltip.
    /// </value>
    public IChartTooltip? Tooltip { get; protected set; }

    /// <summary>
    /// Gets the animations speed.
    /// </summary>
    /// <value>
    /// The animations speed.
    /// </value>
    public TimeSpan AnimationsSpeed { get; protected set; }

    /// <summary>
    /// Gets the easing function.
    /// </summary>
    /// <value>
    /// The easing function.
    /// </value>
    public Func<float, float>? EasingFunction { get; protected set; }

    /// <summary>
    /// Gets the visual elements.
    /// </summary>
    /// <value>
    /// The visual elements.
    /// </value>
    public IEnumerable<ChartElement> VisualElements { get; protected set; } =
        [];

    internal event Action<Chart, LvcPoint>? PointerDown;
    internal event Action<Chart, LvcPoint>? PointerUp;
    internal event Action<Chart, LvcPoint>? PointerMove;

    internal bool DisableTooltipCache { get; set; }

    #endregion region

    /// <summary>
    /// Updates the chart.
    /// </summary>
    /// <param name="chartUpdateParams">The update params.</param>
    public virtual void Update(ChartUpdateParams? chartUpdateParams = null)
    {
        chartUpdateParams ??= new ChartUpdateParams();

        if (chartUpdateParams.IsAutomaticUpdate && !View.AutoUpdateEnabled) return;

        _updateThrottler.ThrottlerTimeSpan = View.UpdaterThrottler;

        if (!chartUpdateParams.Throttling)
        {
            _updateThrottler.ForceCall();
            return;
        }

        _updateThrottler.Call();
    }

    /// <summary>
    /// Finds the points near to the specified point.
    /// </summary>
    /// <param name="pointerPosition">The pointer position.</param>
    /// <returns></returns>
    public abstract IEnumerable<ChartPoint> FindHoveredPointsBy(LvcPoint pointerPosition);

    /// <summary>
    /// Loads the control resources.
    /// </summary>
    public virtual void Load()
    {
        IsLoaded = true;
        _isFirstDraw = true;
        View.Tooltip ??= LiveCharts.DefaultSettings.GetTheme().DefaultTooltip();
        View.Legend ??= LiveCharts.DefaultSettings.GetTheme().DefaultLegend();
        Update();
    }

    /// <summary>
    /// Unloads the control.
    /// </summary>
    public virtual void Unload()
    {
        IsLoaded = false;
        _everMeasuredElements.Clear();
        _toDeleteElements.Clear();
        _activePoints.Clear();
        Canvas.Dispose();
    }

    /// <summary>
    /// Invokes the pointer down event.
    /// </summary>
    /// <param name="point">The pointer position.</param>
    /// <param name="isSecondaryAction">Flags the action as secondary (normally rigth click or double tap on mobile)</param>
    protected internal virtual void InvokePointerDown(LvcPoint point, bool isSecondaryAction)
    {
        _isPanning = true;
        _pointerPreviousPanningPosition = point;
        _pointerPanningStartPosition = point;

        lock (Canvas.Sync)
        {
#if NET5_0_OR_GREATER
            if (_isMobile) _isTooltipCanceled = false;
#endif

            var strategy = FindingStrategy;

            if (strategy == FindingStrategy.Automatic)
                strategy = VisibleSeries.GetFindingStrategy();

            // fire the series event.
            foreach (var series in VisibleSeries)
            {
                if (!series.RequiresFindClosestOnPointerDown) continue;

                var points = series.FindHitPoints(this, point, strategy, FindPointFor.PointerDownEvent);
                if (!points.Any()) continue;

                series.OnDataPointerDown(View, points, point);
            }

            // fire the chart event.
            var iterablePoints = VisibleSeries.SelectMany(x => x.FindHitPoints(this, point, strategy, FindPointFor.PointerDownEvent));
            View.OnDataPointerDown(iterablePoints, point);

            // fire the visual elements event.
            var hitElements =
                _everMeasuredElements
                    .OfType<IInternalInteractable>()
                    .Where(x => x.GetHitBox().Contains(point));

            foreach (var ve in hitElements)
                ve.InvokePointerDown(new VisualElementEventArgs(this, ve, point));

            View.OnVisualElementPointerDown(hitElements, point);
        }

        // experimental events from the chart engine.
        PointerDown?.Invoke(this, point);
    }

    /// <summary>
    /// Invokes the pointer move event.
    /// </summary>
    /// <param name="point">The pointer position.</param>
    protected internal virtual void InvokePointerMove(LvcPoint point)
    {
        _pointerPosition = point;
        _isPointerIn = true;
        _tooltipThrottler.Call();

        // experimental events from the chart engine.
        PointerMove?.Invoke(this, point);

        if (!_isPanning) return;
        _pointerPanningPosition = point;
        _panningThrottler.Call();
    }

    /// <summary>
    /// Invokes the pointer up event.
    /// </summary>
    /// <param name="point">The pointer position.</param>
    /// <param name="isSecondaryAction">Flags the action as secondary (normally rigth click or double tap on mobile)</param>
    protected internal virtual void InvokePointerUp(LvcPoint point, bool isSecondaryAction)
    {
#if NET5_0_OR_GREATER
        if (_isMobile)
        {
            lock (Canvas.Sync)
            {
                _isTooltipCanceled = true;
            }

            View.InvokeOnUIThread(CloseTooltip);
        }
#endif

        // experimental events from the chart engine.
        PointerUp?.Invoke(this, point);

        if (!_isPanning) return;
        _isPanning = false;
        _pointerPanningPosition = point;
        _panningThrottler.Call();
    }

    /// <summary>
    /// Invokes the pointer out event.
    /// </summary>
    protected internal virtual void InvokePointerLeft()
    {
        View.OnHoveredPointsChanged(null, _activePoints);
        _ = CleanHoveredPoints([]);

        View.InvokeOnUIThread(CloseTooltip);
        _isPointerIn = false;
    }

    /// <summary>
    /// Measures this chart.
    /// </summary>
    /// <returns></returns>
    protected internal abstract void Measure();

    /// <summary>
    /// Sets the draw margin.
    /// </summary>
    /// <param name="controlSize">Size of the control.</param>
    /// <param name="margin">The margin.</param>
    /// <returns></returns>
    protected void SetDrawMargin(LvcSize controlSize, Margin margin)
    {
        DrawMarginSize = new LvcSize
        {
            Width = controlSize.Width - margin.Left - margin.Right,
            Height = controlSize.Height - margin.Top - margin.Bottom
        };

        DrawMarginLocation = new LvcPoint(margin.Left, margin.Top);
    }

    /// <summary>
    /// Saves the previous size of the chart.
    /// </summary>
    protected void SetPreviousSize() => _previousSize = ControlSize;

    /// <summary>
    /// Invokes the <see cref="Measuring"/> event.
    /// </summary>
    /// <returns></returns>
    protected void InvokeOnMeasuring() => Measuring?.Invoke(View);

    /// <summary>
    /// Invokes the on update started.
    /// </summary>
    /// <returns></returns>
    protected void InvokeOnUpdateStarted()
    {
        SetPreviousSize();
        UpdateStarted?.Invoke(View);
    }

    /// <summary>
    /// Invokes the on update finished.
    /// </summary>
    /// <returns></returns>
    protected void InvokeOnUpdateFinished() => UpdateFinished?.Invoke(View);

    /// <summary>
    /// Returns a value indicating if the control size changed.
    /// </summary>
    /// <returns></returns>
    protected bool SizeChanged() => _previousSize.Width != ControlSize.Width || _previousSize.Height != ControlSize.Height;

    /// <summary>
    /// Is it being rendered
    /// </summary>
    public bool _isRendering = false;

    /// <summary>
    /// Indicates if it is the first render
    /// </summary>
    public bool _isFirstRender = true;

    /// <summary>
    /// Called when the updated the throttler is unlocked.
    /// </summary>
    /// <returns></returns>
    protected virtual Task UpdateThrottlerUnlocked()
    {
        return Task.Run(() =>
        {
            View.InvokeOnUIThread(() =>
            {
                lock (Canvas.Sync)
                {
                    //1.The first rendering must be released (Chart will be added to the subscription list),
                    //otherwise the subsequent UpdateThrottlerUnlocked cannot be triggered (you need to click on the chart to join the subscription again).
                    //2.Allow traffic only when MotionCanvas is being rendered, so as to avoid the infinite accumulation of elements in the chart when it is not rendered.
                    if (_isFirstRender || _isRendering)
                    {
                        _isFirstRender = false;
                        Measure();
                    }
                }
            });
        });
    }

    /// <summary>
    /// Updates the bounds tracker.
    /// </summary>
    protected void UpdateBounds()
    {
        ActualBounds.Location = DrawMarginLocation;
        ActualBounds.Size = DrawMarginSize;

        if (_isFirstDraw)
        {
            ActualBounds.Animate(EasingFunction, AnimationsSpeed);
            _ = Canvas.Trackers.Add(ActualBounds);
        }
    }

    /// <summary>
    /// Initializes the visuals collector.
    /// </summary>
    protected void InitializeVisualsCollector() =>
        _toDeleteElements = new HashSet<IChartElement>(_everMeasuredElements);

    /// <summary>
    /// Adds a visual element to the chart.
    /// </summary>
    public void AddVisual(IChartElement element)
    {
        element.Invalidate(this);
        element.RemoveOldPaints(View);
        _ = _everMeasuredElements.Add(element);
        _ = _toDeleteElements.Remove(element);
    }

    /// <summary>
    /// Removes a visual element from the chart.
    /// </summary>
    public void RemoveVisual(IChartElement element)
    {
        element.RemoveFromUI(this);
        _ = _everMeasuredElements.Remove(element);
        _ = _toDeleteElements.Remove(element);
    }

    /// <summary>
    /// Gets the legend position.
    /// </summary>
    /// <returns>The position.</returns>
    public LvcPoint GetLegendPosition()
    {
        var actualChartSize = ControlSize;
        float x = 0f, y = 0f;

        if (LegendPosition == LegendPosition.Top)
        {
            x = actualChartSize.Width * 0.5f - _legendSize.Width * 0.5f;
            y = _titleHeight;
        }
        if (LegendPosition == LegendPosition.Bottom)
        {
            x = actualChartSize.Width * 0.5f - _legendSize.Width * 0.5f;
            y = actualChartSize.Height - _legendSize.Height;
        }
        if (LegendPosition == LegendPosition.Left)
        {
            x = 0f;
            y = actualChartSize.Height * 0.5f - _legendSize.Height * 0.5f;
        }
        if (LegendPosition == LegendPosition.Right)
        {
            x = actualChartSize.Width - _legendSize.Width;
            y = actualChartSize.Height * 0.5f - _legendSize.Height * 0.5f;
        }

        return new(x, y);
    }

    /// <summary>
    /// Determines whether the specified series is drawn in the UI already for the given chart.
    /// </summary>
    /// <param name="seriesId">The series id.</param>
    /// <returns>A boolean indicating whether the series is drawn.</returns>
    public bool IsDrawn(int seriesId) => _drawnSeries.Contains(seriesId);

    /// <summary>
    /// Collects and deletes from the UI the unused visuals.
    /// </summary>
    protected void CollectVisuals()
    {
        foreach (var visual in _toDeleteElements)
        {
            if (visual is ISeries series)
            {
                // series delete softly and animate as they leave the UI.
                // UPDATE
                // actually series are not even removed sofly.. this is only disposing things
                // and causes bugs such as #1164
                series.SoftDeleteOrDispose(View);
            }
            else
            {
                visual.RemoveFromUI(this);
            }

            _ = _everMeasuredElements.Remove(visual);
        }

        _toDeleteElements = [];
    }

    /// <summary>
    /// Draws the legend and appends the size of the legend to the current margin calculation.
    /// </summary>
    /// <param name="ts">The top margin.</param>
    /// <param name="bs">The bottom margin.</param>
    /// <param name="ls">The left margin.</param>
    /// <param name="rs">The right margin.</param>
    protected void DrawLegend(ref float ts, ref float bs, ref float ls, ref float rs)
    {
        if (Legend is null || LegendPosition == LegendPosition.Hidden)
        {
            Legend?.Hide(this);
            return;
        }

        _legendSize = Legend.Measure(this);

        switch (LegendPosition)
        {
            case LegendPosition.Top: ts += _legendSize.Height; break;
            case LegendPosition.Left: ls += _legendSize.Width; break;
            case LegendPosition.Right: rs += _legendSize.Width; break;
            case LegendPosition.Bottom: bs += _legendSize.Height; break;
            case LegendPosition.Hidden:
            default:
                break;
        }

        Legend.Draw(this);
        _preserveFirstDraw = _isFirstDraw;
    }

    /// <summary>
    /// Draws the current tool tip, requires canvas invalidation after this call.
    /// </summary>
    /// <returns>A value indicating whether the tooltip was drawn.</returns>
    protected bool DrawToolTip()
    {
        var x = _pointerPosition.X;
        var y = _pointerPosition.Y;

        if (Tooltip is null || !_isPointerIn ||
            x < DrawMarginLocation.X || x > DrawMarginLocation.X + DrawMarginSize.Width ||
            y < DrawMarginLocation.Y || y > DrawMarginLocation.Y + DrawMarginSize.Height)
        {
            return false;
        }

        var hovered = new HashSet<ChartPoint>(FindHoveredPointsBy(_pointerPosition));
        var added = new HashSet<ChartPoint>();

        foreach (var point in hovered)
        {
            if (_activePoints.Contains(point)) continue;

            point.Context.Series.OnPointerEnter(point);

            _ = _activePoints.Add(point);
            _ = added.Add(point);
        }

        var removed = CleanHoveredPoints(hovered);

        var tooltipDataChanged =
            added.Count > 0 || removed.Count > 0 || DisableTooltipCache;

        if (tooltipDataChanged)
            View.OnHoveredPointsChanged(added, removed);

        if (hovered.Count == 0 || TooltipPosition == TooltipPosition.Hidden)
        {
            _isToolTipOpen = false;
            Tooltip?.Hide(this);
            return false;
        }

        if (tooltipDataChanged)
        {
            Tooltip?.Show(hovered, this);
            _isToolTipOpen = true;
        }

        return true;
    }

    private List<ChartPoint> CleanHoveredPoints(HashSet<ChartPoint> hovered)
    {
        var removed = new List<ChartPoint>();

        IEnumerable<ChartPoint> active = _activePoints;

#if NET5_0_OR_GREATER
#else
        active = active.ToArray();
#endif

        foreach (var point in active)
        {
            if (hovered.Contains(point)) continue;

            point.Context.Series.OnPointerLeft(point);
            _ = _activePoints.Remove(point);
            removed.Add(point);
        }

        return removed;
    }

    private Task TooltipThrottlerUnlocked()
    {
        return Task.Run(() =>
             View.InvokeOnUIThread(() =>
             {
                 lock (Canvas.Sync)
                 {
#if NET5_0_OR_GREATER
                     if (_isTooltipCanceled) return;
#endif
                     var tooltipDrawn = DrawToolTip();
                     if (!tooltipDrawn) return;

                     Canvas.Invalidate();
                 }
             }));
    }

    private Task PanningThrottlerUnlocked()
    {
        return Task.Run(() =>
            View.InvokeOnUIThread(() =>
            {
                if (this is not CartesianChartEngine cartesianChart) return;

                lock (Canvas.Sync)
                {
                    var dx = _pointerPanningPosition.X - _pointerPreviousPanningPosition.X;
                    var dy = _pointerPanningPosition.Y - _pointerPreviousPanningPosition.Y;

                    cartesianChart.Pan(new LvcPoint(dx, dy), _isPanning);
                    _pointerPreviousPanningPosition = new LvcPoint(_pointerPanningPosition.X, _pointerPanningPosition.Y);
                }
            }));
    }

    private void CloseTooltip()
    {
        _isToolTipOpen = false;
        Tooltip?.Hide(this);
        _ = CleanHoveredPoints([]);

        if (this is CartesianChartEngine cartesianChart)
        {
            foreach (var ax in cartesianChart.XAxes) ax.ClearCrosshair(cartesianChart);
            foreach (var ay in cartesianChart.YAxes) ay.ClearCrosshair(cartesianChart);
        }
    }

    private void OnCanvasValidated(CoreMotionCanvas chart) => InvokeOnUpdateFinished();
}
