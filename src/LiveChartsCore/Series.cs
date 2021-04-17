// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a series in a Cartesian chart.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="LiveChartsCore.ISeries" />
    /// <seealso cref="LiveChartsCore.ISeries{TModel}" />
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class Series<TModel, TVisual, TLabel, TDrawingContext> : ISeries, ISeries<TModel>, IDisposable, INotifyPropertyChanged
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
        private readonly CollectionDeepObserver<TModel> observer;
        private readonly SeriesProperties properties;
        private IEnumerable<TModel>? values;
        private string? name;
        private Action<TModel, ChartPoint>? mapping;
        private int zIndex;
        private Func<ChartPoint, string> tooltipLabelFormatter = (point) => $"{point.Context.Series.Name} {point.PrimaryValue}"; 
        private Func<ChartPoint, string> dataLabelsFormatter = (point) => $"{point.PrimaryValue}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public Series(SeriesProperties properties)
        {
            this.properties = properties;
            observer = new CollectionDeepObserver<TModel>(
                (sender, e) => NotifySubscribers(),
                (sender, e) => NotifySubscribers());
        }

        /// <inheritdoc />
        public SeriesProperties SeriesProperties => properties;

        /// <inheritdoc />
        public string? Name { get => name; set { name = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data set to draw in the chart.
        /// </summary>
        public IEnumerable<TModel>? Values
        {
            get => values;
            set
            {
                observer.Dispose(values);
                observer.Initialize(value);
                values = value;
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
        public Action<TModel, ChartPoint>? Mapping { get => mapping; set { mapping = value; OnPropertyChanged(); } }

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
        /// Occurs when aa property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a delegate that will be called everytime a <see cref="ChartPoint"/> instance
        /// is added to a state.
        /// </summary>
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointAddedToState { get; set; }

        /// <summary>
        /// Gets or sets a delegate that will be called everytime a <see cref="ChartPoint"/> instance
        /// is removed from a state.
        /// </summary>
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointRemovedFromState { get; set; }

        /// <inheritdoc cref="ISeries.ZIndex" />
        public int ZIndex { get => zIndex; set { zIndex = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ISeries.TooltipLabelFormatter" />
        public Func<ChartPoint, string> TooltipLabelFormatter { get => tooltipLabelFormatter; set { tooltipLabelFormatter = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ISeries.DataLabelsFormatter" />
        public Func<ChartPoint, string> DataLabelsFormatter { get => dataLabelsFormatter; set { dataLabelsFormatter = value; OnPropertyChanged(); } }

        /// <inheritdoc />
        public virtual int GetStackGroup() => 0;

        /// <inheritdoc cref="ISeries.Fetch(IChart)"/>
        protected IEnumerable<ChartPoint> Fetch(IChart chart)
        {
            if (dataProvider == null) throw new Exception("Data provider not found");
            subscribedTo.Add(chart);
            return dataProvider.Fetch(this, chart);
        }

        IEnumerable<ChartPoint> ISeries.Fetch(IChart chart) => Fetch(chart);

        ///// <inheritdoc />
        //IEnumerable<ChartPoint> ISeries.Fetch(IChart chart) => Fetch(chart);

        /// <inheritdoc />
        IEnumerable<TooltipPoint> ISeries.FindPointsNearTo(IChart chart, PointF pointerPosition) =>
            FilterTooltipPoints(Fetch(chart), chart, pointerPosition);

        /// <inheritdoc />
        public void AddPointToState(ChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            if (chart.PointStates == null) return;

            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null) return;
            //throw new Exception(
            //    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)} property is null, " +
            //    $"this is probably due the point was not measured yet.");

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(ChartPoint)}.{nameof(ChartPoint.Context)}.{nameof(ChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnAddedToState(visual, chart);

            if (s.Fill != null) s.Fill.AddGeometyToPaintTask(highlitable);
            if (s.Stroke != null) s.Stroke.AddGeometyToPaintTask(highlitable);
        }

        /// <inheritdoc />
        public virtual void RemovePointFromState(ChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null) return;
            //throw new Exception(
            //    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)} property is null, " +
            //    $"this is probably due the point was not measured yet.");

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(ChartPoint)}.{nameof(ChartPoint.Context)}.{nameof(ChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnRemovedFromState(visual, chart);

            if (s.Fill != null) s.Fill.RemoveGeometryFromPainTask(highlitable);
            if (s.Stroke != null) s.Stroke.RemoveGeometryFromPainTask(highlitable);
        }

        /// <inheritdoc cref="ISeries.Delete"/>
        public virtual void Delete(IChartView chart) { }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
            foreach (var chart in subscribedTo) Delete(chart.View);
            observer.Dispose(values);
        }

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
            => (OnPointAddedToState ?? DefaultOnPointAddedToSate)(visual, chart);

        /// <summary>
        /// Called when a point was removed from a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected void OnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart)
            => (OnPointRemovedFromState ?? DefaultOnRemovedFromState)(visual, chart);

        /// <summary>
        /// Defines de default behaviour when a point is added to a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected virtual void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart) { }

        /// <summary>
        /// Defines the default behavious when a point is remvoed from a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected virtual void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) { }

        /// <summary>
        /// Gets the label position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="labelSize">Size of the label.</param>
        /// <param name="position">The position.</param>
        /// <param name="seriesProperties">The series properties.</param>
        /// <param name="isGreaterThanPivot">if set to <c>true</c> [is greater than pivot].</param>
        /// <returns></returns>
        protected virtual PointF GetLabelPosition(
            float x,
            float y,
            float width,
            float height,
            SizeF labelSize,
            DataLabelsPosition position,
            SeriesProperties seriesProperties,
            bool isGreaterThanPivot)
        {
            var middleX = (x + x + width) * 0.5f;
            var middleY = (y + y + height) * 0.5f;

            return position switch
            {
                DataLabelsPosition.Top => new PointF(middleX, y - labelSize.Height * 0.5f),
                DataLabelsPosition.Bottom => new PointF(middleX, y + height + labelSize.Height * 0.5f),
                DataLabelsPosition.Left => new PointF(x - labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.Right => new PointF(x + width + labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.Middle => new PointF(middleX, middleY),
                _ => (seriesProperties & SeriesProperties.HorizontalOrientation) == SeriesProperties.HorizontalOrientation
                    ? position switch
                    {
                        DataLabelsPosition.End => isGreaterThanPivot
                            ? new PointF(x + width + labelSize.Width * 0.5f, middleY)
                            : new PointF(x - labelSize.Width * 0.5f, middleY),
                        DataLabelsPosition.Start => isGreaterThanPivot
                            ? new PointF(x - labelSize.Width * 0.5f, middleY)
                            : new PointF(x + width + labelSize.Width * 0.5f, middleY),
                        _ => throw new NotImplementedException(),
                    }
                    : position switch
                    {
                        DataLabelsPosition.End => isGreaterThanPivot
                            ? new PointF(middleX, y - labelSize.Height * 0.5f)
                            : new PointF(middleX, y + height + labelSize.Height * 0.5f),
                        DataLabelsPosition.Start => isGreaterThanPivot
                            ? new PointF(middleX, y + height + labelSize.Height * 0.5f)
                            : new PointF(middleX, y - labelSize.Height * 0.5f),
                        _ => throw new NotImplementedException(),
                    }
            };
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<TooltipPoint> FilterTooltipPoints(
            IEnumerable<ChartPoint>? points, IChart chart, PointF pointerPosition)
        {
            if (points == null) yield break;

            foreach (var point in points)
            {
                if (point == null || point.Context.HoverArea == null) continue;
                if (!point.Context.HoverArea.IsTriggerBy(pointerPosition, chart.TooltipFindingStrategy)) continue;

                yield return new TooltipPoint(this, point);
            }
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }
    }
}
