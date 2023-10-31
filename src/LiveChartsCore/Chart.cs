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
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="IChart" />
public abstract class Chart<TDrawingContext> : IChart
    where TDrawingContext : DrawingContext
{
    #region fields

    internal readonly HashSet<ChartElement<TDrawingContext>> _everMeasuredElements = new();
    internal HashSet<ChartElement<TDrawingContext>> _toDeleteElements = new();
    internal bool _isToolTipOpen = false;
    internal bool _isPointerIn;
    internal LvcPoint _pointerPosition = new(-10, -10);
    internal float _titleHeight = 0f;
    internal LvcSize _legendSize;
    internal bool _preserveFirstDraw = false;
    internal readonly HashSet<int> _drawnSeries = new();
    internal bool _isFirstDraw = true;
    private readonly ActionThrottler _updateThrottler;
    private readonly ActionThrottler _tooltipThrottler;
    private readonly ActionThrottler _panningThrottler;
    private bool _isTooltipCanceled;
    private LvcPoint _pointerPanningStartPosition = new(-10, -10);
    private LvcPoint _pointerPanningPosition = new(-10, -10);
    private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
    private bool _isPanning = false;
    private readonly Dictionary<ChartPoint, object> _activePoints = new();
    private LvcSize _previousSize = new();
    private readonly bool _isMobile;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="defaultPlatformConfig">The default platform configuration.</param>
    /// <param name="view">The chart view.</param>
    protected Chart(
        MotionCanvas<TDrawingContext> canvas,
        Action<LiveChartsSettings> defaultPlatformConfig,
        IChartView view)
    {
        Canvas = canvas;
        canvas.Validated += OnCanvasValidated;
        EasingFunction = EasingFunctions.QuadraticOut;
        LiveCharts.Configure(defaultPlatformConfig);

        _updateThrottler = view.DesignerMode
                ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
        _updateThrottler.ThrottlerTimeSpan = view.UpdaterThrottler;

        _tooltipThrottler = new ActionThrottler(TooltipThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
        _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));

#if NET5_0_OR_GREATER

        _isMobile = OperatingSystem.IsOSPlatform("Android") || OperatingSystem.IsOSPlatform("iOS");

#endif
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<TDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<TDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<TDrawingContext>? UpdateFinished;

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
    /// Gets or sets the theme identifier.
    /// </summary>
    /// <value>
    /// The theme identifier.
    /// </value>
    public object ThemeId { get; protected set; } = new();

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
    public MotionCanvas<TDrawingContext> Canvas { get; private set; }

    /// <summary>
    /// Gets the visible series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public abstract IEnumerable<IChartSeries<TDrawingContext>> VisibleSeries { get; }

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public abstract IEnumerable<IChartSeries<TDrawingContext>> Series { get; }

    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public abstract IChartView<TDrawingContext> View { get; }

    IChartView IChart.View => View;

    /// <summary>
    /// The series context
    /// </summary>
    public SeriesContext<TDrawingContext> SeriesContext { get; protected set; } = new(Enumerable.Empty<IChartSeries<TDrawingContext>>(), null!);

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
    public IChartLegend<TDrawingContext>? Legend { get; protected set; }

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
    public TooltipFindingStrategy TooltipFindingStrategy { get; protected set; }

    /// <summary>
    /// Gets the tooltip.
    /// </summary>
    /// <value>
    /// The tooltip.
    /// </value>
    public IChartTooltip<TDrawingContext>? Tooltip { get; protected set; }

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
    public IEnumerable<ChartElement<TDrawingContext>> VisualElements { get; protected set; } =
        Array.Empty<ChartElement<TDrawingContext>>();

    object IChart.Canvas => Canvas;

    #endregion region

    /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
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

    internal virtual void InvokePointerDown(LvcPoint point, bool isSecondaryAction)
    {
        _isPanning = true;
        _pointerPreviousPanningPosition = point;
        _pointerPanningStartPosition = point;

        lock (Canvas.Sync)
        {
            if (_isMobile) _isTooltipCanceled = false;

            var strategy = VisibleSeries.GetTooltipFindingStrategy();

            // fire the series event.
            foreach (var series in VisibleSeries)
            {
                if (!series.RequiresFindClosestOnPointerDown) continue;

                var points = series.FindHitPoints(this, point, strategy);
                if (!points.Any()) continue;

                series.OnDataPointerDown(View, points, point);
            }

            // fire the chart event.
            var iterablePoints = VisibleSeries.SelectMany(x => x.FindHitPoints(this, point, strategy));
            View.OnDataPointerDown(iterablePoints, point);

            // fire the visual elements event.
            var hitElements =
                _everMeasuredElements.OfType<VisualElement<TDrawingContext>>()
                    .Cast<VisualElement<TDrawingContext>>()
                    .SelectMany(x => x.IsHitBy(this, point));

            foreach (var ve in hitElements)
                ve.InvokePointerDown(new VisualElementEventArgs<TDrawingContext>(this, ve, point));

            View.OnVisualElementPointerDown(hitElements, point);
        }
    }

    internal virtual void InvokePointerMove(LvcPoint point)
    {
        _pointerPosition = point;
        _isPointerIn = true;
        _tooltipThrottler.Call();

        if (!_isPanning) return;
        _pointerPanningPosition = point;
        _panningThrottler.Call();
    }

    internal virtual void InvokePointerUp(LvcPoint point, bool isSecondaryAction)
    {
        if (_isMobile)
        {
            lock (Canvas.Sync)
            {
                _isTooltipCanceled = true;
            }

            View.InvokeOnUIThread(CloseTooltip);
        }

        if (!_isPanning) return;
        _isPanning = false;
        _pointerPanningPosition = point;
        _panningThrottler.Call();
    }

    internal void InvokePointerLeft()
    {
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
    protected void SetPreviousSize()
    {
        _previousSize = ControlSize;
    }

    /// <summary>
    /// Invokes the <see cref="Measuring"/> event.
    /// </summary>
    /// <returns></returns>
    protected void InvokeOnMeasuring()
    {
        Measuring?.Invoke(View);
    }

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
    protected void InvokeOnUpdateFinished()
    {
        UpdateFinished?.Invoke(View);
    }

    /// <summary>
    /// Returns a value indicating if the control size changed.
    /// </summary>
    /// <returns></returns>
    protected bool SizeChanged()
    {
        return _previousSize.Width != ControlSize.Width || _previousSize.Height != ControlSize.Height;
    }

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
                    Measure();
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
    protected void InitializeVisualsCollector()
    {
        _toDeleteElements = new HashSet<ChartElement<TDrawingContext>>(_everMeasuredElements);
    }

    /// <summary>
    /// Adds a visual element to the chart.
    /// </summary>
    public void AddVisual(ChartElement<TDrawingContext> element)
    {
        element.Invalidate(this);
        element.RemoveOldPaints(View);
        _ = _everMeasuredElements.Add(element);
        _ = _toDeleteElements.Remove(element);
    }

    /// <summary>
    /// Removes a visual element from the chart.
    /// </summary>
    public void RemoveVisual(ChartElement<TDrawingContext> element)
    {
        element.RemoveFromUI(this);
        _ = _everMeasuredElements.Remove(element);
        _ = _toDeleteElements.Remove(element);
    }

    /// <summary>
    /// Gets the legend position.
    /// </summary>
    /// <returns>The position of the legend.</returns>
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
    /// Collects and deletes from the UI the unused visuals.
    /// </summary>
    protected void CollectVisuals()
    {
        foreach (var visual in _toDeleteElements)
        {
            if (visual is ISeries series)
            {
                // series delete softly and animate as they leave the UI.
                series.SoftDeleteOrDispose(View);
            }
            else
            {
                visual.RemoveFromUI(this);
            }

            _ = _everMeasuredElements.Remove(visual);
        }

        _toDeleteElements = new HashSet<ChartElement<TDrawingContext>>();
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
        if (Legend is null || LegendPosition == LegendPosition.Hidden) return;

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
    protected void DrawToolTip()
    {
        var x = _pointerPosition.X;
        var y = _pointerPosition.Y;

        if (Tooltip is null || TooltipPosition == TooltipPosition.Hidden || !_isPointerIn ||
            x < DrawMarginLocation.X || x > DrawMarginLocation.X + DrawMarginSize.Width ||
            y < DrawMarginLocation.Y || y > DrawMarginLocation.Y + DrawMarginSize.Height)
        {
            return;
        }

        var points = FindHoveredPointsBy(_pointerPosition);

        var o = new object();
        var isEmpty = true;
        foreach (var tooltipPoint in points)
        {
            tooltipPoint.Context.Series.OnPointerEnter(tooltipPoint);
            _activePoints[tooltipPoint] = o;
            isEmpty = false;
        }

        CleanHoveredPoints(o);

        if (isEmpty) return;

        Tooltip?.Show(points, this);
        _isToolTipOpen = true;
    }

    private void CleanHoveredPoints(object comparer)
    {
        foreach (var point in _activePoints.Keys.ToArray())
        {
            if (_activePoints[point] == comparer) continue; // the points was used, don't remove it.

            point.Context.Series.OnPointerLeft(point);
            _ = _activePoints.Remove(point);
        }
    }

    private Task TooltipThrottlerUnlocked()
    {
        return Task.Run(() =>
             View.InvokeOnUIThread(() =>
             {
                 lock (Canvas.Sync)
                 {
                     if (_isTooltipCanceled) return;
                     DrawToolTip();
                     Canvas.Invalidate();
                 }
             }));
    }

    private Task PanningThrottlerUnlocked()
    {
        return Task.Run(() =>
            View.InvokeOnUIThread(() =>
            {
                if (this is not CartesianChart<TDrawingContext> cartesianChart) return;

                lock (Canvas.Sync)
                {
                    var dx = _pointerPanningPosition.X - _pointerPreviousPanningPosition.X;
                    var dy = _pointerPanningPosition.Y - _pointerPreviousPanningPosition.Y;

                    // we need to send a dummy value indicating the direction (val > 0)
                    // so the core is able to bounce the panning when the user reaches the limit.
                    if (dx == 0) dx = _pointerPanningStartPosition.X - _pointerPanningPosition.X > 0 ? -0.01f : 0.01f;
                    if (dy == 0) dy = _pointerPanningStartPosition.Y - _pointerPanningPosition.Y > 0 ? -0.01f : 0.01f;

                    cartesianChart.Pan(new LvcPoint(dx, dy), _isPanning);
                    _pointerPreviousPanningPosition = new LvcPoint(_pointerPanningPosition.X, _pointerPanningPosition.Y);
                }
            }));
    }

    private void CloseTooltip()
    {
        _isToolTipOpen = false;
        Tooltip?.Hide(this);
        CleanHoveredPoints(new());

        if (this is CartesianChart<TDrawingContext> cartesianChart)
        {
            foreach (var ax in cartesianChart.XAxes) ax.ClearCrosshair(cartesianChart);
            foreach (var ay in cartesianChart.YAxes) ay.ClearCrosshair(cartesianChart);
        }
    }

    private void OnCanvasValidated(MotionCanvas<TDrawingContext> chart)
    {
        InvokeOnUpdateFinished();
    }
}
