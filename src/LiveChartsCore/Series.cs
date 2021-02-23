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
using System.Linq;

namespace LiveChartsCore
{
    public abstract class Series<TModel, TVisual, TDrawingContext> : IDataSeries<TDrawingContext>, IDisposable
        where TDrawingContext : DrawingContext
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        private readonly CollectionDeepObserver<TModel> observerer;
        private readonly object measuredFor = new object();
        private IEnumerable<IChartPoint> fetched = Enumerable.Empty<IChartPoint>();
        protected readonly bool isValueType;
        protected readonly bool implementsICP;
        protected readonly Dictionary<int, ChartPoint<TModel>> byValueVisualMap = new Dictionary<int, ChartPoint<TModel>>();
        protected readonly Dictionary<TModel, ChartPoint<TModel>> byReferenceVisualMap = new Dictionary<TModel, ChartPoint<TModel>>();
        private readonly HashSet<CartesianChartCore<TDrawingContext>> subscribedTo = new HashSet<CartesianChartCore<TDrawingContext>>();
        private readonly SeriesProperties properties;
        private IEnumerable<TModel>? values;
        protected PaintContext<TDrawingContext> paintContext = new PaintContext<TDrawingContext>();
        private IDrawableTask<TDrawingContext>? stroke = null;
        private IDrawableTask<TDrawingContext>? fill = null;
        private IDrawableTask<TDrawingContext>? highlightStroke = null;
        private IDrawableTask<TDrawingContext>? highlightFill = null;
        private double legendShapeSize = 15;

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
            implementsICP = typeof(IChartPoint).IsAssignableFrom(t);
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

        public IDrawableTask<TDrawingContext>? Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;
                }

                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? Fill
        {
            get => fill;
            set
            {
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;
                    fill.StrokeWidth = 0;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? HighlightStroke
        {
            get => highlightStroke;
            set
            {
                highlightStroke = value;
                if (highlightStroke != null)
                {
                    highlightStroke.IsStroke = true;
                    highlightStroke.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? HighlightFill
        {
            get => highlightFill;
            set
            {
                highlightFill = value;
                if (highlightFill != null)
                {
                    highlightFill.IsStroke = false;
                    highlightFill.StrokeWidth = 0;
                    highlightFill.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        public ChartPointMapperDelegate<TModel>? Mapping { get; set; }

        /// <inheritdoc/>
        public virtual IEnumerable<IChartPoint> Fetch(CartesianChartCore<TDrawingContext> chart)
        {
            subscribedTo.Add(chart);
            if (measuredFor == chart.MeasureWorker) return fetched;

            fetched = implementsICP ? GetPointsFromICP() : GetMappedPoints();

            return fetched;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            observerer.Dispose(values);
        }

        protected virtual void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)legendShapeSize, Width = (float)legendShapeSize };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = (float)legendShapeSize,
                    Width = (float)legendShapeSize
                };
                w += 2 * strokeClone.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }

        protected virtual void OnPointMeasured(IChartPoint chartPoint, TVisual visual)
        {
        }

        private IEnumerable<IChartPoint> GetPointsFromICP()
        {
            if (values == null) yield break;

            var index = 0;

            foreach (var item in values)
            {
                var chartPoint = (IChartPoint)item;
                chartPoint.PointContext = new ChartPointContext(index++, item, properties, true);
                yield return chartPoint;
            }
        }

        private IEnumerable<IChartPoint> GetMappedPoints()
        {
            if (values == null) yield break;

            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapper<TModel>();
            var index = 0;

            if (isValueType)
            {
                foreach (var item in values)
                {
                    if (!byValueVisualMap.TryGetValue(index, out ChartPoint<TModel> cp))
                        byValueVisualMap[index] = (cp = new ChartPoint<TModel>());

                    cp.PointContext = new ChartPointContext(index++, item, properties, true);
                    mapper(cp, item, cp.PointContext);

                    yield return cp;
                }
            }
            else
            {
                foreach (var item in values)
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out ChartPoint<TModel> cp))
                        byReferenceVisualMap[item] = (cp = new ChartPoint<TModel>());

                    cp.PointContext = new ChartPointContext(index++, item, properties, true);
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
