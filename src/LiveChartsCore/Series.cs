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

// Ignore Spelling: Hoverable Tooltip

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Providers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
namespace LiveChartsCore;

/// <summary>
/// Defines a series in a Cartesian chart.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="ISeries" />
/// <seealso cref="ISeries{TModel}" />
/// <seealso cref="IDisposable" />
/// <seealso cref="INotifyPropertyChanged" />
public abstract class Series<TModel, TVisual, TLabel, TDrawingContext>
    : ChartElement<TDrawingContext>, ISeries, ISeries<TModel>, IInternalSeries, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
        where TVisual : class, IGeometry<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    /// <summary>
    /// The subscribed to
    /// </summary>
    protected readonly HashSet<IChart> subscribedTo = new();

    /// <summary>
    /// The implements icp
    /// </summary>
    protected readonly bool implementsICP;

    /// <summary>
    /// The pivot
    /// </summary>
    protected float pivot = 0f;

    /// <summary>
    /// The max series stroke.
    /// </summary>
    protected const float MAX_MINIATURE_STROKE_WIDTH = 3.5f;

    /// <summary>
    /// The ever fetched points.
    /// </summary>
    protected internal HashSet<ChartPoint> everFetched = new();

    /// <summary>
    /// Indicates whether the custom measure handler was requested already.
    /// </summary>
    protected bool _requestedCustomMeasureHandler = false;

    /// <summary>
    /// The custom measure handler.
    /// </summary>
    protected Action<Chart<TDrawingContext>>? _customMeasureHandler = null;

    /// <summary>
    /// Indicates whether the geometry svg changed.
    /// </summary>
    protected bool _geometrySvgChanged = false;

    /// <summary>
    /// Will be deleted on future versions
    /// </summary>
    [Obsolete]
    protected Func<ChartPoint<TModel, TVisual, TLabel>, string>? _obsolete_formatter = null;

    private readonly CollectionDeepObserver<TModel> _observer;
    private IEnumerable<TModel>? _values;
    private string? _name;
    private Func<TModel, int, Coordinate>? _mapping;
    private int _zIndex;
    private Func<ChartPoint<TModel, TVisual, TLabel>, string> _dataLabelsFormatter = x => x.Coordinate.PrimaryValue.ToString();
    private bool _isVisible = true;
    private LvcPoint _dataPadding = new(0.5f, 0.5f);
    private DataFactory<TModel, TDrawingContext>? _dataFactory;
    private bool _isVisibleAtLegend = true;
    private double _miniatureShapeSize = 12;
    private Sketch<TDrawingContext> _miniatureSketch = new(0, 0, null);
    private Func<float, float>? _easingFunction;
    private TimeSpan? _animationsSpeed;
    private string? _geometrySvg;

    /// <summary>
    /// Initializes a new instance of the <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    protected Series(SeriesProperties properties)
    {
        SeriesProperties = properties;

        if (typeof(ISvgPath<TDrawingContext>).IsAssignableFrom(typeof(TVisual)))
            SeriesProperties |= SeriesProperties.IsSVGPath;

        _observer = new CollectionDeepObserver<TModel>(
            (sender, e) => NotifySubscribers(),
            (sender, e) => NotifySubscribers());
    }

    /// <inheritdoc cref="ISeries.SeriesProperties"/>
    public SeriesProperties SeriesProperties { get; internal set; }

    SeriesProperties IInternalSeries.SeriesProperties
    {
        get => SeriesProperties;
        set => SeriesProperties = value;
    }

    /// <inheritdoc cref="ISeries.Name"/>
    public string? Name { get => _name; set => SetProperty(ref _name, value); }

    /// <summary>
    /// Gets or sets the data set to draw in the chart.
    /// </summary>
    public IEnumerable<TModel>? Values
    {
        get => _values;
        set
        {
            _observer?.Dispose(_values);
            _observer?.Initialize(value);
            _values = value;
            OnPropertyChanged();
        }
    }

    IEnumerable? ISeries.Values { get => Values; set => Values = (IEnumerable<TModel>?)value; }

    /// <inheritdoc cref="ISeries.Pivot"/>
    public double Pivot { get => pivot; set => SetProperty(ref pivot, (float)value); }

    /// <inheritdoc cref="ISeries{TModel}.Mapping"/>
    public Func<TModel, int, Coordinate>? Mapping { get => _mapping; set => SetProperty(ref _mapping, value); }

    /// <inheritdoc cref="ISeries.GeometrySvg"/>
    public string? GeometrySvg
    {
        get => _geometrySvg;
        set
        {
            _geometrySvgChanged = true;
            SetProperty(ref _geometrySvg, value);

            if (!this.HasSvgGeometry())
            {
                throw new Exception(
                    $"You must use a geometry that implements {nameof(ISvgPath<TDrawingContext>)}, " +
                    $"{nameof(TVisual)} does not satisfies the constrait.");
            }
        }
    }

    int ISeries.SeriesId { get; set; } = -1;

    bool ISeries.RequiresFindClosestOnPointerDown => DataPointerDown is not null || ChartPointPointerDown is not null;

    /// <summary>
    /// Occurs when an instance of <see cref="ChartPoint"/> is measured.
    /// </summary>
    public event Action<ChartPoint<TModel, TVisual, TLabel>>? PointMeasured;

    /// <summary>
    /// Occurs when an instance of <see cref="ChartPoint"/> is created.
    /// </summary>
    public event Action<ChartPoint<TModel, TVisual, TLabel>>? PointCreated;

    /// <summary>
    /// Occurs when the pointer goes down over a chart point(s).
    /// </summary>
    public event ChartPointsHandler<TModel, TVisual, TLabel>? DataPointerDown;

    /// <summary>
    /// Occurs when the pointer is over a chart point.
    /// </summary>
    [Obsolete($"Renamed to {nameof(ChartPointPointerHover)}")]
    public event ChartPointHandler<TModel, TVisual, TLabel>? DataPointerHover;

    /// <summary>
    /// Occurs when the pointer left a chart point.
    /// </summary>
    [Obsolete($"Renamed to {nameof(ChartPointPointerHoverLost)}")]
    public event ChartPointHandler<TModel, TVisual, TLabel>? DataPointerHoverLost;

    /// <summary>
    /// Occurs when the pointer is over a chart point.
    /// </summary>
    public event ChartPointHandler<TModel, TVisual, TLabel>? ChartPointPointerHover;

    /// <summary>
    /// Occurs when the pointer left a chart point.
    /// </summary>
    public event ChartPointHandler<TModel, TVisual, TLabel>? ChartPointPointerHoverLost;

    /// <summary>
    /// Occurs when the pointer goes down over a chart point, if there are multiple points, the closest one will be selected.
    /// </summary>
    public event ChartPointHandler<TModel, TVisual, TLabel>? ChartPointPointerDown;

    /// <inheritdoc cref="ISeries.ZIndex" />
    public int ZIndex { get => _zIndex; set => SetProperty(ref _zIndex, value); }

    /// <summary>
    /// Gets or sets the tool tip label formatter, this function will build the label when a point in this series 
    /// is shown inside a tool tip.
    /// </summary>
    /// <value>
    /// The tool tip label formatter.
    /// </value>
    [Obsolete(
        $"You must now use {nameof(CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>.XToolTipLabelFormatter)} or " +
        $"{nameof(CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>.YToolTipLabelFormatter)} instead.")]
    public Func<ChartPoint<TModel, TVisual, TLabel>, string>? TooltipLabelFormatter
    {
        get => _obsolete_formatter;
        set => _obsolete_formatter = value;
    }

    /// <summary>
    /// Gets or sets the data label formatter, this function will build the label when a point in this series 
    /// is shown as data label.
    /// </summary>
    /// <value>
    /// The data label formatter.
    /// </value>
    public Func<ChartPoint<TModel, TVisual, TLabel>, string> DataLabelsFormatter
    {
        get => _dataLabelsFormatter;
        set => SetProperty(ref _dataLabelsFormatter, value);
    }

    /// <inheritdoc cref="ISeries.IsVisible" />
    public bool IsVisible
    {
        get => _isVisible;
        set
        => SetProperty(ref _isVisible, value);
    }

    /// <inheritdoc cref="ISeries.IsHoverable" />
    public bool IsHoverable { get; set; } = true;

    /// <inheritdoc cref="ISeries.IsVisibleAtLegend" />
    public bool IsVisibleAtLegend { get => _isVisibleAtLegend; set => SetProperty(ref _isVisibleAtLegend, value); }

    /// <inheritdoc cref="ISeries.DataPadding" />
    public LvcPoint DataPadding { get => _dataPadding; set => SetProperty(ref _dataPadding, value); }

    /// <inheritdoc cref="ISeries.AnimationsSpeed" />
    public TimeSpan? AnimationsSpeed { get => _animationsSpeed; set => SetProperty(ref _animationsSpeed, value); }

    /// <inheritdoc cref="ISeries.EasingFunction" />
    public Func<float, float>? EasingFunction { get => _easingFunction; set => SetProperty(ref _easingFunction, value); }

    /// <summary>
    /// Gets the data factory.
    /// </summary>
    public DataFactory<TModel, TDrawingContext> DataFactory
    {
        get
        {
            if (_dataFactory is null)
            {
                var factory = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>();
                _dataFactory = factory.GetDefaultDataFactory<TModel>();
            }

            return _dataFactory;
        }
    }

    /// <summary>
    /// Called when a point is measured.
    /// </summary>
    public Action<ChartPoint<TModel, TVisual, TLabel>>? WhenPointMeasured { get; set; }

    /// <summary>
    /// Gets or sets the size of the legend shape.
    /// </summary>
    /// <value>
    /// The size of the legend shape.
    /// </value>
    [Obsolete($"Renamed to {nameof(MiniatureShapeSize)}")]
    public double LegendShapeSize
    {
        get => MiniatureShapeSize;
        set => MiniatureShapeSize = value;
    }

    /// <summary>
    /// Gets or sets the size of the legend shape.
    /// </summary>
    /// <value>
    /// The size of the legend shape.
    /// </value>
    public double MiniatureShapeSize
    {
        get => _miniatureShapeSize;
        set
        {
            _miniatureShapeSize = value;
            OnMiniatureChanged();
            SetProperty(ref _miniatureShapeSize, value);
        }
    }

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.CanvasSchedule"/>
    public Sketch<TDrawingContext> CanvasSchedule
    {
        get => _miniatureSketch;
        protected set => SetProperty(ref _miniatureSketch, value);
    }

    /// <inheritdoc cref="ISeries.VisibilityChanged"/>
    public event Action<ISeries>? VisibilityChanged;

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.GetStackGroup"/>
    public virtual int GetStackGroup()
    {
        return 0;
    }

    /// <inheritdoc cref="ISeries.Fetch(IChart)"/>
    protected IEnumerable<ChartPoint> Fetch(IChart chart)
    {
        _ = subscribedTo.Add(chart);
        return DataFactory.Fetch(this, chart);
    }

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.OnDataPointerDown(IChartView, IEnumerable{ChartPoint}, LvcPoint)"/>
    protected virtual void OnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(chart, points.Select(point => new ChartPoint<TModel, TVisual, TLabel>(point)));
        ChartPointPointerDown?.Invoke(chart, new ChartPoint<TModel, TVisual, TLabel>(points.FindClosestTo<TModel, TVisual, TLabel>(pointer)!));
    }

    IEnumerable<ChartPoint> ISeries.Fetch(IChart chart)
    {
        return Fetch(chart);
    }

    IEnumerable<ChartPoint> ISeries.FindHitPoints(IChart chart, LvcPoint pointerPosition, TooltipFindingStrategy strategy)
    {
        var query =
            Fetch(chart)
            .Where(x =>
                x.Context.HoverArea is not null &&
                x.Context.HoverArea.IsPointerOver(pointerPosition, strategy));

        var s = (int)strategy;
        if (s is >= 4 and <= 6)
        {
            // if select closest...
            query = query
                .Select(x => new { distance = x.DistanceTo(pointerPosition), point = x })
                .OrderBy(x => x.distance)
                .SelectFirst(x => x.point);
        }

        return query;
    }

    void ISeries.OnPointerEnter(ChartPoint point)
    {
        OnPointerEnter(point);
    }

    void ISeries.OnPointerLeft(ChartPoint point)
    {
        OnPointerLeft(point);
    }

    /// <inheritdoc cref="ISeries.RestartAnimations"/>
    public void RestartAnimations()
    {
        if (DataFactory is null) throw new Exception("Data provider not found");
        DataFactory.RestartVisuals();
    }

    /// <inheritdoc cref="ISeries.GetPrimaryToolTipText(ChartPoint)"/>
    public abstract string? GetPrimaryToolTipText(ChartPoint point);

    /// <inheritdoc cref="ISeries.GetSecondaryToolTipText(ChartPoint)"/>
    public abstract string? GetSecondaryToolTipText(ChartPoint point);

    /// <inheritdoc cref="ISeries.GetDataLabelText(ChartPoint)"/>
    public string? GetDataLabelText(ChartPoint point)
    {
        return DataLabelsFormatter is null ? null : DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        base.RemoveFromUI(chart);
        DataFactory?.Dispose(chart);
        _dataFactory = null;
        everFetched = new HashSet<ChartPoint>();
    }

    /// <summary>
    /// Converts a chart to a strong-typed version of it.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public ChartPoint<TModel, TVisual, TLabel> ConvertToTypedChartPoint(ChartPoint point)
    {
        return new ChartPoint<TModel, TVisual, TLabel>(point);
    }

    /// <inheritdoc cref="ISeries.SoftDeleteOrDispose"/>
    public abstract void SoftDeleteOrDispose(IChartView chart);

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.GetMiniaturesSketch"/>
    public abstract Sketch<TDrawingContext> GetMiniaturesSketch();

    /// <summary>
    /// Builds a paint schedule.
    /// </summary>
    /// <param name="paint"></param>
    /// <param name="geometry"></param>
    /// <returns></returns>
    protected PaintSchedule<TDrawingContext> BuildMiniatureSchedule(
        IPaint<TDrawingContext> paint, ISizedGeometry<TDrawingContext> geometry)
    {
        var paintClone = paint.CloneTask();
        var st = paint.IsStroke ? paint.StrokeThickness : 0;

        if (st > MAX_MINIATURE_STROKE_WIDTH)
        {
            st = MAX_MINIATURE_STROKE_WIDTH;
            paintClone.StrokeThickness = MAX_MINIATURE_STROKE_WIDTH;
        }

        geometry.X = 0.5f * st;
        geometry.Y = 0.5f * st;
        geometry.Height = (float)MiniatureShapeSize;
        geometry.Width = (float)MiniatureShapeSize;

        if (paint.IsStroke) paintClone.ZIndex = 1;

        return new PaintSchedule<TDrawingContext>(paintClone, geometry);
    }

    /// <summary>
    /// Called when a point was measured.
    /// </summary>
    /// <param name="chartPoint">The chart point.</param>
    protected internal virtual void OnPointMeasured(ChartPoint chartPoint)
    {
        WhenPointMeasured?.Invoke(new ChartPoint<TModel, TVisual, TLabel>(chartPoint));
        PointMeasured?.Invoke(new ChartPoint<TModel, TVisual, TLabel>(chartPoint));
    }

    /// <summary>
    /// Called when a point is created.
    /// </summary>
    /// <param name="chartPoint">The chart point.</param>
    protected internal virtual void OnPointCreated(ChartPoint chartPoint)
    {
        SetDefaultPointTransitions(chartPoint);
        PointCreated?.Invoke(new ChartPoint<TModel, TVisual, TLabel>(chartPoint));
    }

    /// <summary>
    /// Sets the default point transitions.
    /// </summary>
    /// <param name="chartPoint">The chart point.</param>
    /// <returns></returns>
    protected abstract void SetDefaultPointTransitions(ChartPoint chartPoint);

    /// <summary>
    /// Called when the visibility changes.
    /// </summary>
    protected virtual void OnVisibilityChanged()
    {
        VisibilityChanged?.Invoke(this);
    }

    /// <summary>
    /// Called when the pointer enters a point.
    /// </summary>
    /// <param name="point">The chart point.</param>
    protected virtual void OnPointerEnter(ChartPoint point)
    {
        DataPointerHover?.Invoke(point.Context.Chart, new ChartPoint<TModel, TVisual, TLabel>(point));
        ChartPointPointerHover?.Invoke(point.Context.Chart, new ChartPoint<TModel, TVisual, TLabel>(point));
    }

    /// <summary>
    /// Called when the pointer leaves a point.
    /// </summary>
    /// <param name="point">The chart point.</param>
    protected virtual void OnPointerLeft(ChartPoint point)
    {
        DataPointerHoverLost?.Invoke(point.Context.Chart, new ChartPoint<TModel, TVisual, TLabel>(point));
        ChartPointPointerHoverLost?.Invoke(point.Context.Chart, new ChartPoint<TModel, TVisual, TLabel>(point));
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.OnPaintChanged(string?)"/>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnMiniatureChanged();
    }

    /// <summary>
    /// Called when the miniature changes.
    /// </summary>
    protected void OnMiniatureChanged()
    {
        CanvasSchedule = GetMiniaturesSketch();
    }

    private void NotifySubscribers()
    {
        foreach (var chart in subscribedTo) chart.Update();
    }
}
