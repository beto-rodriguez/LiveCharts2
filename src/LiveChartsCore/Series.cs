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
        where TVisual : class, IHighlightableGeometry<TDrawingContext>, new()
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
        public ChartPointMapperDelegate<TModel>? Mapping { get; set; }

        /// <inheritdoc/>
        public virtual IEnumerable<IChartPoint<TVisual, TDrawingContext>> Fetch(IChart chart)
        {
            if (fetchedFor == chart.MeasureWorker) return fetched;

            subscribedTo.Add(chart);
            fetched = implementsICP ? GetPointsFromICP() : GetMappedPoints();
            fetchedFor = chart.MeasureWorker;

            return fetched;
        }

        IEnumerable<IChartPoint> ISeries.Fetch(IChart chart) => Fetch(chart);

        IEnumerable<TooltipPoint> ISeries.FindPointsNearTo(IChart chart, PointF pointerPosition) =>
            Fetch(chart)
                .Where(point =>
                    point.PointContext.HoverArea.IsTriggerBy(pointerPosition, chart.TooltipFindingStrategy)
                )
                .Select(point => 
                    new TooltipPoint(this, point)
                );

        /// <inheritdoc/>
        public void Dispose()
        {
            observerer.Dispose(values);
        }

        protected virtual void OnPointMeasured(IChartPoint<TVisual, TDrawingContext> chartPoint, TVisual visual)
        {
        }

        private IEnumerable<IChartPoint<TVisual, TDrawingContext>> GetPointsFromICP()
        {
            if (values == null) yield break;

            var index = 0;

            foreach (var item in values)
            {
                // it will never be null at this point, becuase in the constructor
                // we forced the implementation of ICP
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                var cp = (IChartPoint<TVisual, TDrawingContext>)item;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                cp.PointContext.Index = index++;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                cp.PointContext.DataSource = item;
                yield return cp;
            }
        }

        private IEnumerable<IChartPoint<TVisual, TDrawingContext>> GetMappedPoints()
        {
            if (values == null) yield break;

            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapper<TModel>();
            var index = 0;

            if (isValueType)
            {
                foreach (var item in values)
                {
                    if (!byValueVisualMap.TryGetValue(index, out ChartPoint<TModel, TVisual, TDrawingContext> cp))
                        byValueVisualMap[index] = (cp = new ChartPoint<TModel, TVisual, TDrawingContext>());

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
                        byReferenceVisualMap[item] = (cp = new ChartPoint<TModel, TVisual, TDrawingContext>());

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

        public abstract int GetStackGroup();
    }
}
