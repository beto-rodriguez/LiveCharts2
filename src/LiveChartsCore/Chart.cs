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
using System.Diagnostics;
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

    internal bool _preserveFirstDraw = false;
    private readonly ActionThrottler _updateThrottler;
    private readonly ActionThrottler _tooltipThrottler;
    private readonly ActionThrottler _panningThrottler;
    private LvcPoint _pointerPosition = new(-10, -10);
    private LvcPoint _pointerPanningStartPosition = new(-10, -10);
    private LvcPoint _pointerPanningPosition = new(-10, -10);
    private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
    private bool _isPanning = false;
    private bool _isPointerIn = false;
    private readonly Dictionary<ChartPoint, object> _activePoints = new();
    private LvcSize _previousSize = new();

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
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(defaultPlatformConfig);

        _updateThrottler = view.DesignerMode
                ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));

        PointerDown += Chart_PointerDown;
        PointerMove += Chart_PointerMove;
        PointerUp += Chart_PointerUp;
        PointerLeft += Chart_PointerLeft;

        _tooltipThrottler = new ActionThrottler(TooltipThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
        _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<TDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<TDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<TDrawingContext>? UpdateFinished;

    internal event Action<LvcPoint> PointerDown;

    internal event Action<LvcPoint> PointerMove;

    internal event Action<LvcPoint> PointerUp;

    internal event Action PointerLeft;

    internal event Action<PanGestureEventArgs>? PanGesture;

    #region properties

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
    /// Gets or sets a value indicating whether this it is the first draw of this instance.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this it is the first draw; otherwise, <c>false</c>.
    /// </value>
    public bool IsFirstDraw { get; internal set; } = true;

    /// <summary>
    /// Gets the canvas.
    /// </summary>
    /// <value>
    /// The canvas.
    /// </value>
    public MotionCanvas<TDrawingContext> Canvas { get; private set; }

    /// <summary>
    /// Gets the drawable series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public abstract IEnumerable<IChartSeries<TDrawingContext>> ChartSeries { get; }

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
    public SeriesContext<TDrawingContext> SeriesContext { get; protected set; } = new(Enumerable.Empty<IChartSeries<TDrawingContext>>());

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
    /// Gets the legend orientation.
    /// </summary>
    /// <value>
    /// The legend orientation.
    /// </value>
    public LegendOrientation LegendOrientation { get; protected set; }

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
    /// Gets or sets the updater throttler.
    /// </summary>
    /// <value>
    /// The updater throttler.
    /// </value>
    public TimeSpan UpdaterThrottler
    {
        get => _updateThrottler.ThrottlerTimeSpan;
        set => _updateThrottler.ThrottlerTimeSpan = value;
    }

    /// <summary>
    /// Gets the visual elements.
    /// </summary>
    /// <value>
    /// The visual elements.
    /// </value>
    public ChartElement<TDrawingContext>[] VisualElements { get; protected set; } = Array.Empty<ChartElement<TDrawingContext>>();

    /// <summary>
    /// Gets the previous legend position.
    /// </summary>
    public LegendPosition PreviousLegendPosition { get; protected set; }

    /// <summary>
    /// Gets the previous series.
    /// </summary>
    public IReadOnlyList<IChartSeries<TDrawingContext>> PreviousSeriesAtLegend { get; protected set; } = Array.Empty<IChartSeries<TDrawingContext>>();

    object IChart.Canvas => Canvas;

    #endregion region

    /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
    public virtual void Update(ChartUpdateParams? chartUpdateParams = null)
    {
        chartUpdateParams ??= new ChartUpdateParams();

        if (chartUpdateParams.IsAutomaticUpdate && !View.AutoUpdateEnabled) return;

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
        IsFirstDraw = true;
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

    internal void ClearTooltipData()
    {
        foreach (var point in _activePoints.Keys.ToArray())
        {
            point.Context.Series.OnPointerLeft(point);
            _ = _activePoints.Remove(point);
        }

        Canvas.Invalidate();
    }
    internal virtual void InvokePointerDown(LvcPoint point, bool isSecondaryAction)
    {
        PointerDown?.Invoke(point);

        var strategy = ChartSeries.GetTooltipFindingStrategy();

        // fire the series event.
        foreach (var series in ChartSeries)
        {
            if (!series.RequiresFindClosestOnPointerDown) continue;

            var points = series.FindHitPoints(this, point, strategy);
            if (!points.Any()) continue;

            series.OnDataPointerDown(View, points, point);
        }

        // fire the chart event.
        var iterablePoints = ChartSeries.SelectMany(x => x.FindHitPoints(this, point, strategy));
        View.OnDataPointerDown(iterablePoints, point);

        // fire the visual elements event.
        // ToDo: VisualElements should be of type VisualElement<T>
        var iterableVisualElements = VisualElements.Cast<VisualElement<TDrawingContext>>().SelectMany(x => x.IsHitBy(point));
        View.OnVisualElementPointerDown(iterableVisualElements, point);
    }

    internal virtual void InvokePointerMove(LvcPoint point)
    {
        PointerMove?.Invoke(point);
    }

    internal virtual void InvokePointerUp(LvcPoint point, bool isSecondaryAction)
    {
        PointerUp?.Invoke(point);
    }

    internal void InvokePointerLeft()
    {
        PointerLeft?.Invoke();
    }

    internal void InvokePanGestrue(PanGestureEventArgs eventArgs)
    {
        PanGesture?.Invoke(eventArgs);
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
    ///SDetermines whether the series miniature changed or not.
    /// </summary>
    /// <param name="newSeries">The new series.</param>
    /// <param name="position">The legend position.</param>
    /// <returns></returns>
    protected virtual bool SeriesMiniatureChanged(IReadOnlyList<IChartSeries<TDrawingContext>> newSeries, LegendPosition position)
    {
        if (position == LegendPosition.Hidden && PreviousLegendPosition == LegendPosition.Hidden) return false;
        if (position != PreviousLegendPosition) return true;
        if (PreviousSeriesAtLegend.Count != newSeries.Count) return true;

        for (var i = 0; i < newSeries.Count; i++)
        {
            if (i + 1 > PreviousSeriesAtLegend.Count) return true;

            var a = PreviousSeriesAtLegend[i];
            var b = newSeries[i];

            if (!a.MiniatureEquals(b)) return true;
        }

        return false;
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
    /// Updates thhe bounds tracker.
    /// </summary>
    protected void UpdateBounds()
    {
        ActualBounds.Location = DrawMarginLocation;
        ActualBounds.Size = DrawMarginSize;

        if (IsFirstDraw)
        {
            _ = ActualBounds
                .TransitionateProperties(null)
                .WithAnimation(animation =>
                         animation
                             .WithDuration(AnimationsSpeed)
                             .WithEasingFunction(EasingFunction))
                .CompleteCurrentTransitions();

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
    /// Registers and tracks the specified element.
    /// </summary>
    public void RegisterAndInvalidateVisual(ChartElement<TDrawingContext> element)
    {
        element.Invalidate(this);
        element.RemoveOldPaints(View);
        _ = _everMeasuredElements.Add(element);
        _ = _toDeleteElements.Remove(element);
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

    private Task TooltipThrottlerUnlocked()
    {
        return Task.Run(() =>
             View.InvokeOnUIThread(() =>
             {
                 lock (Canvas.Sync)
                 {
#if DEBUG
                     if (LiveCharts.EnableLogging)
                     {
                         Trace.WriteLine(
                             $"[tooltip view thread]".PadRight(60) +
                             $"tread: {Environment.CurrentManagedThreadId}");
                     }
#endif

                     if (Tooltip is null || TooltipPosition == TooltipPosition.Hidden || !_isPointerIn) return;

                     // TODO:
                     // all this needs a performance review...
                     // it should not be crital, should not be even close to be the 'bottle neck' in a case where
                     // we face perfomance issues.

                     var points = FindHoveredPointsBy(_pointerPosition).ToArray();

                     if (!points.Any())
                     {
                         ClearTooltipData();
                         Tooltip.Hide();
                         return;
                     }

                     if (_activePoints.Count > 0 && points.All(x => _activePoints.ContainsKey(x))) return;

                     var o = new object();
                     foreach (var tooltipPoint in points)
                     {
                         tooltipPoint.Context.Series.OnPointerEnter(tooltipPoint);
                         _activePoints[tooltipPoint] = o;
                     }

                     foreach (var point in _activePoints.Keys.ToArray())
                     {
                         if (_activePoints[point] == o) continue;
                         point.Context.Series.OnPointerLeft(point);
                         _ = _activePoints.Remove(point);
                     }

                     Tooltip.Show(points, this);

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

    private void OnCanvasValidated(MotionCanvas<TDrawingContext> chart)
    {
        InvokeOnUpdateFinished();
    }

    private void Chart_PointerDown(LvcPoint pointerPosition)
    {
        _isPanning = true;
        _pointerPreviousPanningPosition = pointerPosition;
        _pointerPanningStartPosition = pointerPosition;
    }

    private void Chart_PointerMove(LvcPoint pointerPosition)
    {
        _pointerPosition = pointerPosition;
        _isPointerIn = true;
        if (Tooltip is not null && TooltipPosition != TooltipPosition.Hidden) _tooltipThrottler.Call();
        if (!_isPanning) return;
        _pointerPanningPosition = pointerPosition;
        _panningThrottler.Call();
    }

    private void Chart_PointerLeft()
    {
        _isPointerIn = false;
    }

    private void Chart_PointerUp(LvcPoint pointerPosition)
    {
        if (!_isPanning) return;
        _isPanning = false;
        _pointerPanningPosition = pointerPosition;
        _panningThrottler.Call();
    }
}
