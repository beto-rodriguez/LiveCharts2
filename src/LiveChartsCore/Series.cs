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
    public abstract class Series<TModel, TVisual, TLabel, TDrawingContext> : ISeries, IDisposable
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private readonly CollectionDeepObserver<TModel> observer;
        private object fetchedFor = new object();
        private ChartPoint<TModel, TVisual, TLabel, TDrawingContext>[] fetched = new ChartPoint<TModel, TVisual, TLabel, TDrawingContext>[0];
        protected readonly bool isValueType;
        protected readonly bool implementsICP;
        protected readonly Dictionary<int, ChartPoint<TModel, TVisual, TLabel, TDrawingContext>> byValueVisualMap = new Dictionary<int, ChartPoint<TModel, TVisual, TLabel, TDrawingContext>>();
        protected readonly Dictionary<TModel, ChartPoint<TModel, TVisual, TLabel, TDrawingContext>> byReferenceVisualMap = new Dictionary<TModel, ChartPoint<TModel, TVisual, TLabel, TDrawingContext>>();
        private readonly HashSet<IChart> subscribedTo = new HashSet<IChart>();
        private readonly SeriesProperties properties;
        private IEnumerable<TModel>? values;

        public Series(SeriesProperties properties)
        {
            this.properties = properties;
            observer = new CollectionDeepObserver<TModel>(
                (object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    NotifySubscribers();
                },
                (object sender, PropertyChangedEventArgs e) =>
                {
                    NotifySubscribers();
                });
            var t = typeof(TModel);
            implementsICP = typeof(IChartPoint<TVisual, TLabel, TDrawingContext>).IsAssignableFrom(t);
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
                observer.Dispose(values);
                observer.Initialize(value);
                values = value;
            }
        }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        public Action<IChartPoint, TModel, IChartPointContext>? Mapping { get; set; }

        int ISeries.SeriesId { get; set; } = -1;
        public string HoverState { get; set; } = "Unknown";

        public Action<TVisual, IChartView<TDrawingContext>>? OnPointCreated { get; set; }
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointAddedToState { get; set; }
        public Action<TVisual, IChartView<TDrawingContext>>? OnPointRemovedFromState { get; set; }

        public virtual int GetStackGroup() => 0;

        /// <inheritdoc/>
        public virtual ChartPoint<TModel, TVisual, TLabel, TDrawingContext>[] Fetch(IChart chart)
        {
            if (fetchedFor == chart.MeasureWorker) return fetched;

            subscribedTo.Add(chart);
            fetched = implementsICP
                ? GetPointsFromICP(chart).ToArray() // this method is experimental, it should improve performance, but how much? that is the question...
                : GetMappedPoints(chart).ToArray();
            fetchedFor = chart.MeasureWorker;

            return fetched;
        }

        IChartPoint[] ISeries.Fetch(IChart chart) => Fetch(chart);

        IEnumerable<TooltipPoint> ISeries.FindPointsNearTo(IChart chart, PointF pointerPosition) =>
            FilterTooltipPoints(Fetch(chart), chart, pointerPosition);

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
            observer.Dispose(values);
        }

        protected virtual void OnPointMeasured(IChartPoint<TVisual, TLabel, TDrawingContext> chartPoint, TVisual visual) { }

        protected void OnAddedToState(TVisual visual, IChartView<TDrawingContext> chart) => (OnPointAddedToState ?? DefaultOnPointAddedToSate)(visual, chart);

        protected void OnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) => (OnPointRemovedFromState ?? DefaultOnRemovedFromState)(visual, chart);

        protected virtual void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart) { }

        protected virtual void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart) { }

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

            if ((seriesProperties & SeriesProperties.VerticalOrientation) == SeriesProperties.VerticalOrientation)
            {
                return position switch
                {
                    DataLabelsPosition.End => isGreaterThanPivot
                        ? new PointF(middleX, y - labelSize.Height * 0.5f)
                        : new PointF(middleX, y + height + labelSize.Height * 0.5f),
                    DataLabelsPosition.Start => isGreaterThanPivot
                        ? new PointF(middleX, y)
                        : new PointF(middleX, y + height),
                    DataLabelsPosition.Middle => new PointF(middleX, middleY),
                    _ => throw new NotImplementedException(),
                };
            }
            else
            {
                return position switch
                {
                    DataLabelsPosition.End => isGreaterThanPivot
                        ? new PointF(middleX - labelSize.Width * 0.5f, y - labelSize.Height * 0.5f)
                        : new PointF(middleX - labelSize.Width * 0.5f, y + height + labelSize.Height * 0.5f),
                    DataLabelsPosition.Start => isGreaterThanPivot
                        ? new PointF(middleX, y)
                        : new PointF(middleX, y + height),
                    DataLabelsPosition.Middle => new PointF(middleX, middleY),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private IEnumerable<ChartPoint<TModel, TVisual, TLabel, TDrawingContext>> GetPointsFromICP(IChart chart)
        {
            throw new NotImplementedException();

            //if (values == null) yield break;

            //var index = 0;

            //foreach (var item in values)
            //{
            //    var cp = (IChartPoint<TVisual, TDrawingContext>)item;
            //    cp.Context.Index = index++;
            //    cp.Context.DataSource = item;
            //    yield return cp;
            //}
        }

        private IEnumerable<ChartPoint<TModel, TVisual, TLabel, TDrawingContext>> GetMappedPoints(IChart chart)
        {
            if (values == null) yield break;

            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
            var index = 0;

            if (isValueType)
            {
                foreach (var item in values)
                {
                    if (!byValueVisualMap.TryGetValue(index, out var cp))
                        byValueVisualMap[index] = cp = new ChartPoint<TModel, TVisual, TLabel, TDrawingContext>(chart.View, this);

                    cp.Context.Index = index++;
                    cp.Context.DataSource = item;
                    mapper(cp, item, cp.Context);

                    yield return cp;
                }
            }
            else
            {
                foreach (var item in values)
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out var cp))
                        byReferenceVisualMap[item] = cp = new ChartPoint<TModel, TVisual, TLabel, TDrawingContext>(chart.View, this);

                    cp.Context.Index = index++;
                    cp.Context.DataSource = item;
                    mapper(cp, item, cp.Context);

                    yield return cp;
                }
            }
        }

        private IEnumerable<TooltipPoint> FilterTooltipPoints(ChartPoint<TModel, TVisual, TLabel, TDrawingContext>?[] points, IChart chart, PointF pointerPosition)
        {
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
