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
using System.Drawing;
using LiveChartsCore.Measure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

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
    public abstract class Series<TModel, TVisual, TLabel, TDrawingContext> : PaintableElement<TDrawingContext>, ISeries, ISeries<TModel>, INotifyPropertyChanged
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
        /// The data provider
        /// </summary>
        protected DataProvider<TModel, TDrawingContext>? dataProvider;

        /// <summary>
        /// The ever fetched
        /// </summary>
        protected readonly HashSet<ChartPoint> everFetched = new();
        private readonly CollectionDeepObserver<TModel> _observer;
        private IEnumerable<TModel>? _values;
        private string? _name;
        private Action<TModel, ChartPoint>? _mapping;
        private int _zIndex;
        private Func<ChartPoint, string> _tooltipLabelFormatter = (point) => $"{point.Context.Series.Name} {point.PrimaryValue}";
        private Func<ChartPoint, string> _dataLabelsFormatter = (point) => $"{point.PrimaryValue}";
        private bool _isVisible = true;
        private PointF _dataPadding = new(0.5f, 0.5f);

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public Series(SeriesProperties properties)
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

        /// <inheritdoc />
        public string HoverState { get; set; } = "Unknown";

        /// <summary>
        /// Occurs when an instance of <see cref="ChartPoint"/> is measured.
        /// </summary>
        public event Action<TypedChartPoint<TVisual, TLabel, TDrawingContext>>? PointMeasured;

        /// <summary>
        /// Occurs when an instance of <see cref="ChartPoint"/> is created.
        /// </summary>
        public event Action<TypedChartPoint<TVisual, TLabel, TDrawingContext>>? PointCreated;

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc cref="ISeries.Disposing" />
        public event Action<ISeries>? Disposing;

        /// <summary>
        /// Gets or sets a delegate that will be called every time a <see cref="ChartPoint"/> instance
        /// is added to a state.
        /// </summary>
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointAddedToState { get; set; }

        /// <summary>
        /// Gets or sets a delegate that will be called every time a <see cref="ChartPoint"/> instance
        /// is removed from a state.
        /// </summary>
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointRemovedFromState { get; set; }

        /// <inheritdoc cref="ISeries.ZIndex" />
        public int ZIndex { get => _zIndex; set { _zIndex = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ISeries.TooltipLabelFormatter" />
        public Func<ChartPoint, string> TooltipLabelFormatter
        {
            get => _tooltipLabelFormatter;
            set { _tooltipLabelFormatter = value; OnPropertyChanged(); }
        }

        /// <inheritdoc cref="ISeries.DataLabelsFormatter" />
        public Func<ChartPoint, string> DataLabelsFormatter
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
                _isVisible = value;
                if (!_isVisible) RestartAnimations();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ISeries.DataPadding" />
        public PointF DataPadding { get => _dataPadding; set { _dataPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ISeries.AnimationsSpeed" />
        public TimeSpan? AnimationsSpeed { get; set; }

        /// <inheritdoc cref="ISeries.EasingFunction" />
        public Func<float, float>? EasingFunction { get; set; }

        /// <inheritdoc cref="ISeries.IsNotifyingChanges"/>
        bool ISeries.IsNotifyingChanges { get; set; }

        /// <inheritdoc />
        public virtual int GetStackGroup()
        {
            return 0;
        }

        /// <inheritdoc cref="ISeries.Fetch(IChart)"/>
        protected IEnumerable<ChartPoint> Fetch(IChart chart)
        {
            if (dataProvider == null) throw new Exception("Data provider not found");
            _ = subscribedTo.Add(chart);
            return dataProvider.Fetch(this, chart);
        }

        IEnumerable<ChartPoint> ISeries.Fetch(IChart chart)
        {
            return Fetch(chart);
        }

        IEnumerable<TooltipPoint> ISeries.FindPointsNearTo(IChart chart, PointF pointerPosition, TooltipFindingStrategy automaticStategy)
        {
            return this is IPieSeries<TDrawingContext> pieSeries && pieSeries.IsFillSeries
                ? Enumerable.Empty<TooltipPoint>()
                : FilterTooltipPoints(Fetch(chart), chart, pointerPosition, automaticStategy);
        }

        /// <inheritdoc />
        public void AddPointToState(ChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            if (chart.PointStates == null ||
                (chart.TooltipPosition == TooltipPosition.Hidden && state == HoverState)) return;

            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null) return;

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(ChartPoint)}.{nameof(ChartPoint.Context)}.{nameof(ChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnAddedToState(visual, chart);

            var core = (Chart<TDrawingContext>)chartPoint.Context.Chart.CoreChart;

            if (s.Fill != null)
            {
                s.Fill.SetClipRectangle(
                    core.Canvas,
                    new RectangleF(core.DrawMarginLocation, core.DrawMarginSize));
                s.Fill.AddGeometryToPaintTask(chart.CoreCanvas, highlitable);
            }
            if (s.Stroke != null)
            {
                s.Stroke.SetClipRectangle(
                    core.Canvas,
                    new RectangleF(core.DrawMarginLocation, core.DrawMarginSize));
                s.Stroke.AddGeometryToPaintTask(chart.CoreCanvas, highlitable);
            }
        }

        /// <inheritdoc />
        public virtual void RemovePointFromState(ChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null) return;

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(ChartPoint)}.{nameof(ChartPoint.Context)}.{nameof(ChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnRemovedFromState(visual, chart);

            if (s.Fill != null) s.Fill.RemoveGeometryFromPainTask(chart.CoreCanvas, highlitable);
            if (s.Stroke != null) s.Stroke.RemoveGeometryFromPainTask(chart.CoreCanvas, highlitable);
        }

        /// <inheritdoc cref="ISeries.RestartAnimations"/>
        public void RestartAnimations()
        {
            if (dataProvider == null) throw new Exception("Data provider not found");
            dataProvider.RestartVisuals();
        }

        /// <inheritdoc cref="ISeries.SoftDelete"/>
        public abstract void SoftDelete(IChartView chart);

        /// <summary>
        /// Softs the delete point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="primaryScale">The primary scale.</param>
        /// <param name="secondaryScale">The secondary scale.</param>
        /// <returns></returns>
        protected virtual void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale) { }

        /// <summary>
        /// Called when a point was measured.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        protected virtual void OnPointMeasured(ChartPoint chartPoint)
        {
            PointMeasured?.Invoke(new TypedChartPoint<TVisual, TLabel, TDrawingContext>(chartPoint));
        }

        /// <summary>
        /// Called when a point is created.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        protected virtual void OnPointCreated(ChartPoint chartPoint)
        {
            SetDefaultPointTransitions(chartPoint);
            PointCreated?.Invoke(new TypedChartPoint<TVisual, TLabel, TDrawingContext>(chartPoint));
        }

        /// <summary>
        /// Sets the default point transitions.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <returns></returns>
        protected abstract void SetDefaultPointTransitions(ChartPoint chartPoint);

        /// <summary>
        /// Called when a point was added to a sate.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected void OnAddedToState(TVisual visual, IChartView<TDrawingContext> chart)
        {
            (OnPointAddedToState ?? DefaultOnPointAddedToSate)(visual, chart);
        }

        /// <summary>
        /// Called when a point was removed from a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected void OnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart)
        {
            (OnPointRemovedFromState ?? DefaultOnRemovedFromState)(visual, chart);
        }

        /// <summary>
        /// Defines the default behavior when a point is added to a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected virtual void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart) { }

        /// <summary>
        /// Defines the default behavior when a point is removed from a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected virtual void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) { }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (!((ISeries)this).IsNotifyingChanges) return;
            NotifySubscribers();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<TooltipPoint> FilterTooltipPoints(
            IEnumerable<ChartPoint>? points, IChart chart, PointF pointerPosition, TooltipFindingStrategy automaticStategy)
        {
            if (points == null) return Enumerable.Empty<TooltipPoint>();
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

            var minD = new Tuple<float, List<TooltipPoint>>(10000, new List<TooltipPoint>());

            foreach (var point in points)
            {
                if (point == null || point.Context.HoverArea == null) continue;
                var d = point.Context.HoverArea.GetDistanceToPoint(pointerPosition, automaticStategy); //chart.TooltipFindingStrategy
                if (d > tolerance || d > minD.Item1) continue;

                if (minD.Item1 == d)
                {
                    // in case of a tie, we add a new point to the same result
                    minD.Item2.Add(new TooltipPoint(this, point));
                    continue;
                }

                minD = new Tuple<float, List<TooltipPoint>>(d, new List<TooltipPoint> { new TooltipPoint(this, point) });
            }

            return minD.Item2;
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }
    }
}
