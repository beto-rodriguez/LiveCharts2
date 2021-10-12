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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using LiveChartsCore.Measure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Providers;

namespace LiveChartsCore
{
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
        : ChartElement<TDrawingContext>, ISeries, ISeries<TModel>, INotifyPropertyChanged
            where TDrawingContext : DrawingContext
            where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
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
        protected const float MaxSeriesStroke = 5f;

        /// <summary>
        /// The ever fetched
        /// </summary>
        protected HashSet<ChartPoint> everFetched = new();

        /// <summary>
        /// The hover paint.
        /// </summary>
        protected IPaint<TDrawingContext>? hoverPaint;

        private readonly CollectionDeepObserver<TModel> _observer;
        private IEnumerable<TModel>? _values;
        private string? _name;
        private Action<TModel, ChartPoint>? _mapping;
        private int _zIndex;
        private Func<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>, string> _tooltipLabelFormatter = (point) => $"{point.Context.Series.Name} {point.PrimaryValue}";
        private Func<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>, string> _dataLabelsFormatter = (point) => $"{point.PrimaryValue}";
        private bool _isVisible = true;
        private LvcPoint _dataPadding = new(0.5f, 0.5f);
        private DataFactory<TModel, TDrawingContext>? _dataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected Series(SeriesProperties properties)
        {
            SeriesProperties = properties;
            _observer = new CollectionDeepObserver<TModel>(
                (sender, e) => NotifySubscribers(),
                (sender, e) => NotifySubscribers());
        }

        /// <inheritdoc />
        public SeriesProperties SeriesProperties { get; }

        /// <inheritdoc />
        public string? Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data set to draw in the chart.
        /// </summary>
        public IEnumerable<TModel>? Values
        {
            get => _values;
            set
            {
                _observer.Dispose(_values);
                _observer.Initialize(value);
                _values = value;
                OnPropertyChanged();
            }
        }

        IEnumerable? ISeries.Values { get => Values; set => Values = (IEnumerable<TModel>?)value; }

        /// <inheritdoc />
        public double Pivot { get => pivot; set { pivot = (float)value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in the chart.
        /// </summary>
        public Action<TModel, ChartPoint>? Mapping { get => _mapping; set { _mapping = value; OnPropertyChanged(); } }

        /// <inheritdoc />
        int ISeries.SeriesId { get; set; } = -1;

        /// <summary>
        /// Occurs when an instance of <see cref="ChartPoint"/> is measured.
        /// </summary>
        public event Action<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>>? PointMeasured;

        /// <summary>
        /// Occurs when an instance of <see cref="ChartPoint"/> is created.
        /// </summary>
        public event Action<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>>? PointCreated;

        /// <summary>
        /// Occurs when the pointer is over a chart point.
        /// </summary>
        public event Action<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>>? PointHovered;

        /// <summary>
        /// Occurs when the pointer left a chart point.
        /// </summary>
        public event Action<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>>? PointHoverLost;

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc cref="ISeries.ZIndex" />
        public int ZIndex { get => _zIndex; set { _zIndex = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the tool tip label formatter, this function will build the label when a point in this series 
        /// is shown inside a tool tip.
        /// </summary>
        /// <value>
        /// The tool tip label formatter.
        /// </value>
        public Func<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>, string> TooltipLabelFormatter
        {
            get => _tooltipLabelFormatter;
            set { _tooltipLabelFormatter = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the data label formatter, this function will build the label when a point in this series 
        /// is shown as data label.
        /// </summary>
        /// <value>
        /// The data label formatter.
        /// </value>
        public Func<TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>, string> DataLabelsFormatter
        {
            get => _dataLabelsFormatter;
            set { _dataLabelsFormatter = value; OnPropertyChanged(); }
        }

        /// <inheritdoc cref="ISeries.IsVisible" />
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                var changed = value != _isVisible;
                _isVisible = value;
                if (!_isVisible) RestartAnimations();
                if (value && !((ISeries)this).IsNotifyingChanges) ((ISeries)this).IsNotifyingChanges = true;
                OnPropertyChanged();
                if (changed) OnVisibilityChanged();
            }
        }

        /// <inheritdoc cref="ISeries.DataPadding" />
        public LvcPoint DataPadding { get => _dataPadding; set { _dataPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ISeries.AnimationsSpeed" />
        public TimeSpan? AnimationsSpeed { get; set; }

        /// <inheritdoc cref="ISeries.EasingFunction" />
        public Func<float, float>? EasingFunction { get; set; }

        /// <inheritdoc cref="IStopNPC.IsNotifyingChanges"/>
        bool IStopNPC.IsNotifyingChanges { get; set; }

        /// <summary>
        /// Gets or sets the data factory.
        /// </summary>
        protected DataFactory<TModel, TDrawingContext> DataFactory
        {
            get
            {
                if (_dataFactory is null)
                {
                    var factory = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>();
                    _dataFactory = factory.GetDefaultDataFactory<TModel>();
                }

                return _dataFactory;
            }
        }

        /// <inheritdoc cref="ISeries.VisibilityChanged"/>
        public event Action<ISeries>? VisibilityChanged;

        /// <inheritdoc />
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

        IEnumerable<ChartPoint> ISeries.Fetch(IChart chart)
        {
            return Fetch(chart);
        }

        TooltipPoint[] ISeries.FindPointsNearTo(IChart chart, LvcPoint pointerPosition, TooltipFindingStrategy automaticStategy)
        {
            return this switch
            {
                IPieSeries<TDrawingContext> pieSeries when pieSeries.IsFillSeries => new TooltipPoint[0],
                IBarSeries<TDrawingContext> barSeries => FilterTooltipPoints(Fetch(chart), chart, pointerPosition, automaticStategy),
                _ => FilterTooltipPoints(Fetch(chart), chart, pointerPosition, automaticStategy)
                    .GroupBy(g => g.PointerDistance)
                    .OrderBy(g => g.Key)
                    .DefaultIfEmpty(Enumerable.Empty<TooltipPoint>())
                    .First()
                    .ToArray(),
            };
        }

        void ISeries.OnPointerEnter(ChartPoint point)
        {
            WhenPointerEnters(point);
        }

        void ISeries.OnPointerLeft(ChartPoint point)
        {
            WhenPointerLeaves(point);
        }

        /// <inheritdoc cref="ISeries.RestartAnimations"/>
        public void RestartAnimations()
        {
            if (DataFactory is null) throw new Exception("Data provider not found");
            DataFactory.RestartVisuals();
        }

        /// <inheritdoc cref="ISeries.GetTooltipText(ChartPoint)"/>
        public string GetTooltipText(ChartPoint point)
        {
            return TooltipLabelFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
        }

        /// <inheritdoc cref="ISeries.GetDataLabelText(ChartPoint)"/>
        public string GetDataLabelText(ChartPoint point)
        {
            return DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
        }

        /// <inheritdoc cref="ISeries.SoftDeleteOrDispose"/>
        public abstract void SoftDeleteOrDispose(IChartView chart);

        /// <summary>
        /// Called when a point was measured.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        protected virtual void OnPointMeasured(ChartPoint chartPoint)
        {
            PointMeasured?.Invoke(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(chartPoint));
        }

        /// <summary>
        /// Called when a point is created.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        protected virtual void OnPointCreated(ChartPoint chartPoint)
        {
            SetDefaultPointTransitions(chartPoint);
            PointCreated?.Invoke(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(chartPoint));
        }

        /// <summary>
        /// Sets the default point transitions.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <returns></returns>
        protected abstract void SetDefaultPointTransitions(ChartPoint chartPoint);

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!((ISeries)this).IsNotifyingChanges) return;
            NotifySubscribers();
        }

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
        /// /// <param name="point">The chart point.</param>
        protected virtual void WhenPointerEnters(ChartPoint point)
        {
            var chart = (IChartView<TDrawingContext>)point.Context.Chart;

            if (hoverPaint is null)
            {
                hoverPaint = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>()
                    .GetSolidColorPaint(new LvcColor(255, 255, 255, 180));
                hoverPaint.ZIndex = int.MaxValue;
                chart.CoreCanvas.AddDrawableTask(hoverPaint);
            }

            var visual = (TVisual?)point.Context.Visual;
            if (visual is null || visual.HighlightableGeometry is null) return;

            hoverPaint.AddGeometryToPaintTask(chart.CoreCanvas, visual.HighlightableGeometry);

            PointHovered?.Invoke(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
        }

        /// <summary>
        /// Called when the pointer leaves a point.
        /// </summary>
        /// /// <param name="point">The chart point.</param>
        protected virtual void WhenPointerLeaves(ChartPoint point)
        {
            if (hoverPaint is null) return;

            var visual = (TVisual?)point.Context.Visual;
            if (visual is null || visual.HighlightableGeometry is null) return;

            hoverPaint.RemoveGeometryFromPainTask(
                (MotionCanvas<TDrawingContext>)point.Context.Chart.CoreChart.Canvas,
                visual.HighlightableGeometry);

            PointHoverLost?.Invoke(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
        }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
        public override void RemoveFromUI(Chart<TDrawingContext> chart)
        {
            base.RemoveFromUI(chart);
            DataFactory?.Dispose(chart);
            _dataFactory = null;
            everFetched = new();
        }

        private TooltipPoint[] FilterTooltipPoints(
            IEnumerable<ChartPoint>? points, IChart chart, LvcPoint pointerPosition, TooltipFindingStrategy automaticStategy)
        {
            if (points is null) return new TooltipPoint[0];
            var tolerance = float.MaxValue;

            if (this is ICartesianSeries<TDrawingContext> cartesianSeries)
            {
                var cartesianChart = (CartesianChart<TDrawingContext>)chart;
                var drawLocation = cartesianChart.DrawMarginLocation;
                var drawMarginSize = cartesianChart.DrawMarginSize;
                var x = cartesianChart.XAxes[cartesianSeries.ScalesXAt];
                var y = cartesianChart.YAxes[cartesianSeries.ScalesYAt];
                var xScale = new Scaler(drawLocation, drawMarginSize, x);
                var yScale = new Scaler(drawLocation, drawMarginSize, y);
                var uwx = xScale.ToPixels((float)x.UnitWidth) - xScale.ToPixels(0);
                var uwy = yScale.ToPixels((float)y.UnitWidth) - yScale.ToPixels(0);

                switch (chart.TooltipFindingStrategy)
                {
                    case TooltipFindingStrategy.CompareAll:
                        tolerance = (float)Math.Sqrt(Math.Pow(uwx, 2) + Math.Pow(uwy, 2));
                        break;
                    case TooltipFindingStrategy.CompareOnlyX:
                        tolerance = Math.Abs(uwx);
                        break;
                    case TooltipFindingStrategy.CompareOnlyY:
                        tolerance = Math.Abs(uwy);
                        break;
                    case TooltipFindingStrategy.Automatic:
                    default:
                        break;
                }
            }

            var distancesT = points
                .Where(point => !(point is null || point.Context.HoverArea is null))
                .Select(point => new TooltipPoint(
                    this, point, point.Context.HoverArea!.GetDistanceToPoint(pointerPosition, automaticStategy)))
                .Where(point => point.PointerDistance < tolerance)
                .OrderBy(dtp => dtp.PointerDistance);

            var lowestD = distancesT.FirstOrDefault()?.PointerDistance;
            var secondLowestD = distancesT.FirstOrDefault(dtp => dtp.PointerDistance > lowestD)?.PointerDistance;

            return distancesT
                .TakeWhile(dtp => dtp.PointerDistance == lowestD || dtp.PointerDistance == secondLowestD)
                .ToArray();
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }
    }
}
