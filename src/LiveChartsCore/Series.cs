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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public abstract class Series<TModel, TVisual, TDrawingContext> : ISeries, IDisposable
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    {
        private readonly CollectionDeepObserver<TModel> observerer;
        private object fetchedFor = new object();
        private IEnumerable<IChartPoint<TVisual, TDrawingContext>> fetched = Enumerable.Empty<IChartPoint<TVisual, TDrawingContext>>();
        protected readonly bool isValueType;
        protected readonly bool implementsICP;
        protected readonly Dictionary<int, ChartPoint<TModel, TVisual, TDrawingContext>> byValueVisualMap = new Dictionary<int, ChartPoint<TModel, TVisual, TDrawingContext>>();
        protected readonly Dictionary<TModel, ChartPoint<TModel, TVisual, TDrawingContext>> byReferenceVisualMap = new Dictionary<TModel, ChartPoint<TModel, TVisual, TDrawingContext>>();
        private readonly HashSet<IChart> subscribedTo = new HashSet<IChart>();
        private readonly SeriesProperties properties;
        private IEnumerable<TModel>? values;

        public Series(SeriesProperties properties)
        {
            this.properties = properties;
            observerer = new CollectionDeepObserver<TModel>(
                (object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    NotifySubscribers();
                },
                (object sender, PropertyChangedEventArgs e) =>
                {
                    NotifySubscribers();
                });
            var t = typeof(TModel);
            implementsICP = typeof(IChartPoint<TVisual, TDrawingContext>).IsAssignableFrom(t);
            isValueType = t.IsValueType;
        }

        public SeriesProperties SeriesProperties => properties;

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the series to draw in the chart.
        /// </summary>
        public IEnumerable<TModel>? Values
        {
            get => values;
            set
            {
                observerer.Dispose(values);
                observerer.Initialize(value);
                values = value;
            }
        }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        public Action<IChartPoint, TModel, IChartPointContext>? Mapping { get; set; }

        int ISeries.SeriesId { get; set; } = -1;
        public string HoverState { get; set; }

        public Action<TVisual, IChartView<TDrawingContext>>? OnPointCreated { get; set; }
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointAddedToState { get; set; }
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointRemovedFromState { get; set; }

        public abstract int GetStackGroup();

        /// <inheritdoc/>
        public virtual IEnumerable<IChartPoint<TVisual, TDrawingContext>> Fetch(IChart chart)
        {
            if (fetchedFor == chart.MeasureWorker) return fetched;

            subscribedTo.Add(chart);
            fetched = implementsICP
                ? GetPointsFromICP(chart) // this method is experimental, it should improve performance, but how much? that is the question...
                : GetMappedPoints(chart);
            fetchedFor = chart.MeasureWorker;

            return fetched;
        }

        IEnumerable<IChartPoint> ISeries.Fetch(IChart chart) => Fetch(chart);

        IEnumerable<TooltipPoint> ISeries.FindPointsNearTo(IChart chart, PointF pointerPosition) =>
            Fetch(chart)
                .Where(point => point.PointContext.HoverArea?.IsTriggerBy(pointerPosition, chart.TooltipFindingStrategy) ?? false)
                .Select(point => new TooltipPoint(this, point));

        public void AddPointToState(IChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            if (chart.PointStates == null) return;

            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null)
                throw new Exception(
                    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnAddedToState(visual, chart);

            if (s.Fill != null) s.Fill.AddGeometyToPaintTask(highlitable);
            if (s.Stroke != null) s.Stroke.AddGeometyToPaintTask(highlitable);
        }

        public virtual void RemovePointFromState(IChartPoint chartPoint, string state)
        {
            var chart = (IChartView<TDrawingContext>)chartPoint.Context.Chart;
            var s = chart.PointStates[state];

            if (s == null)
                throw new Exception($"The state '{state}' was not found");

            if (chartPoint.Context.Visual == null)
                throw new Exception(
                    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            var visual = (TVisual)chartPoint.Context.Visual;
            var highlitable = visual.HighlightableGeometry;

            if (highlitable == null)
                throw new Exception(
                    $"The {nameof(IChartPoint)}.{nameof(IChartPoint.Context)}.{nameof(IChartPoint.Context.Visual)}" +
                    $".{nameof(IVisualChartPoint<TDrawingContext>.HighlightableGeometry)} property is null, " +
                    $"this is probably due the point was not measured yet.");

            OnRemovedFromState(visual, chart);

            if (s.Fill != null) s.Fill.RemoveGeometryFromPainTask(highlitable);
            if (s.Stroke != null) s.Stroke.RemoveGeometryFromPainTask(highlitable);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            observerer.Dispose(values);
        }

        protected virtual void OnPointMeasured(IChartPoint<TVisual, TDrawingContext> chartPoint, TVisual visual) { }

        protected void OnAddedToState(TVisual visual, IChartView<TDrawingContext> chart) => (OnPointAddedToState ?? DefaultOnPointAddedToSate)(visual, chart);

        protected void OnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) => (OnPointRemovedFromState ?? DefaultOnRemovedFromState)(visual, chart);

        protected virtual void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart) { }

        protected virtual void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) { }

        private IEnumerable<IChartPoint<TVisual, TDrawingContext>> GetPointsFromICP(IChart chart)
        {
            if (values == null) yield break;

            var index = 0;

            foreach (var item in values)
            {
                var cp = (IChartPoint<TVisual, TDrawingContext>)item;
                cp.PointContext.Index = index++;
                cp.PointContext.DataSource = item;
                yield return cp;
            }
        }

        private IEnumerable<IChartPoint<TVisual, TDrawingContext>> GetMappedPoints(IChart chart)
        {
            if (values == null) yield break;

            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
            var index = 0;

            if (isValueType)
            {
                foreach (var item in values)
                {
                    if (!byValueVisualMap.TryGetValue(index, out ChartPoint<TModel, TVisual, TDrawingContext> cp))
                        byValueVisualMap[index] = cp = new ChartPoint<TModel, TVisual, TDrawingContext>(chart.View, this);

                    cp.PointContext.Index = index++;
                    cp.PointContext.DataSource = item;
                    mapper(cp, item, cp.PointContext);

                    yield return cp;
                }
            }
            else
            {
                foreach (var item in values)
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out ChartPoint<TModel, TVisual, TDrawingContext> cp))
                        byReferenceVisualMap[item] = cp = new ChartPoint<TModel, TVisual, TDrawingContext>(chart.View, this);

                    cp.PointContext.Index = index++;
                    cp.PointContext.DataSource = item;
                    mapper(cp, item, cp.PointContext);

                    yield return cp;
                }
            }
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }
    }
}
