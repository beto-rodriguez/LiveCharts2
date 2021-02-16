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
    /// <summary>
    /// Defines data to plot in a chart.
    /// </summary>
    public abstract class Series<TModel, TVisual, TDrawingContext> : IDisposable, ISeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        private readonly SeriesType seriesType;
        private readonly SeriesDirection direction;
        private readonly bool isColumnOrRow;
        private readonly HashSet<ChartCore<TDrawingContext>> subscribedTo = new HashSet<ChartCore<TDrawingContext>>();
        private INotifyCollectionChanged previousValuesNCCInstance;
        private IEnumerable<TModel> values;
        protected bool implementsINCC = false;
        protected PaintContext<TDrawingContext> paintContext;
        private CartesianBounds _currentBounds = null;
        protected readonly bool isValueType;
        protected readonly bool implementsINPC;
        protected readonly bool implementsICC;
        protected Dictionary<int, ICartesianCoordinate> byValueVisualMap = new Dictionary<int, ICartesianCoordinate>();
        protected Dictionary<TModel, ICartesianCoordinate> byReferenceVisualMap = new Dictionary<TModel, ICartesianCoordinate>();
        private IDrawableTask<TDrawingContext> stroke;
        private IDrawableTask<TDrawingContext> fill;
        private IDrawableTask<TDrawingContext> highlightStroke;
        private IDrawableTask<TDrawingContext> highlightFill;
        private double legendShapeSize = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{T}"/> class.
        /// </summary>
        public Series(SeriesType type, SeriesDirection direction, bool isColumnOrRow)
        {
            seriesType = type;
            this.direction = direction;
            this.isColumnOrRow = isColumnOrRow;
            var t = typeof(TModel);
            implementsINPC = typeof(INotifyPropertyChanged).IsAssignableFrom(t);
            implementsICC = typeof(ICartesianCoordinate).IsAssignableFrom(t);
            isValueType = t.IsValueType;
        }

        public SeriesType SeriesType => seriesType;
        public SeriesDirection Direction => direction;
        public bool IsColumnOrRow => isColumnOrRow;

        /// <summary>
        /// Gets or sets the series to draw in the chart.
        /// </summary>
        public IEnumerable<TModel> Values
        {
            get => values;
            set
            {
                if (value != previousValuesNCCInstance)
                {
                    if (previousValuesNCCInstance != null) previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
                    if (value is INotifyCollectionChanged incc)
                    {
                        incc.CollectionChanged += OnValuesCollectionChanged;
                        implementsINCC = true;
                    }
                    previousValuesNCCInstance = values as INotifyCollectionChanged;
                    _currentBounds = null;
                }
                values = value;
            }
        }

        /// <inheritdoc/>
        public int ScalesXAt { get; set; }

        /// <inheritdoc/>
        public int ScalesYAt { get; set; }

        public IDrawableTask<TDrawingContext> Stroke
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

        public IDrawableTask<TDrawingContext> Fill
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

        public IDrawableTask<TDrawingContext> HighlightStroke
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

        public IDrawableTask<TDrawingContext> HighlightFill
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

        public string Name { get; set; }

        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }
        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        public Func<TModel, int, ICartesianCoordinate> Mapping { get; set; }

        /// <inheritdoc/>
        public virtual IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart)
        {
            subscribedTo.Add(chart);
            return GetPonts();
        }

        /// <inheritdoc/>
        public virtual CartesianBounds GetBounds(
            SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y, SeriesContext<TDrawingContext> context)
        {
            if (_currentBounds != null && implementsICC && implementsINCC && implementsINPC) return _currentBounds;

            var seriesLength = 0;

            var stack = context.GetStackPosition(this, GetStackGroup());

            // when we implement INotifyCollectionChanged, INotifyPropertyChanged and ICartesianCoordinate
            // then we could skip this the next code.
            var bounds = new CartesianBounds();
            foreach (var coordinate in GetPonts())
            {
                var isXLimit = coordinate.X == bounds.XAxisBounds.max || coordinate.X == bounds.XAxisBounds.min;
                var isYLimit = coordinate.Y == bounds.YAxisBounds.Max || coordinate.Y == bounds.YAxisBounds.min;

                var cx = coordinate.X;
                var cy = coordinate.Y;
                if (stack != null) 
                {
                    var s = stack.StackPoint(coordinate);
                    if (stack.Stacker.Orientation == SeriesDirection.Horizontal) cx = s;
                    else cy = s;
                }

                var abx = bounds.XAxisBounds.AppendValue(cx);
                var aby = bounds.YAxisBounds.AppendValue(cy);

                if (abx > 0)
                {
                    if (!isXLimit) bounds.XCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.XCoordinatesBounds.Add(coordinate);
                }

                if (aby > 0)
                {
                    if (!isYLimit) bounds.YCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.YCoordinatesBounds.Add(coordinate);
                }

                seriesLength++;
            }

            _currentBounds = bounds;
            return bounds;
        }

        /// <inheritdoc/>
        public abstract void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            SeriesContext<TDrawingContext> context,
            HashSet<IDrawable<TDrawingContext>> drawBucket);
        /// <summary>
        /// Gets the 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICartesianCoordinate> GetPonts() => implementsICC ? GetPointsFromICC() : GetMappedPoints();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (previousValuesNCCInstance != null)
                previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
            byReferenceVisualMap = null;
            byValueVisualMap = null;
        }

        protected virtual void OnPointMeasured(ICartesianCoordinate coordinate, TVisual visual)
        {
        }

        private IEnumerable<ICartesianCoordinate> GetPointsFromICC()
        {
            var i = 0;
            foreach (var item in Values.Cast<ICartesianCoordinate>())
            {
                item.Index = i++;
                item.DataSource = item;
                item.PropertyChanged -= OnValuesElementPropertyChanged;
                item.PropertyChanged += OnValuesElementPropertyChanged;
                yield return item;
            }
        }

        private IEnumerable<ICartesianCoordinate> GetMappedPoints()
        {
            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapping<TModel>();
            var index = 0;
            foreach (var item in Values)
            {
                if (implementsINPC)
                {
                    var inpc = (INotifyPropertyChanged)item;
                    inpc.PropertyChanged -= OnValuesElementPropertyChanged;
                    inpc.PropertyChanged += OnValuesElementPropertyChanged;
                }

                ICartesianCoordinate icc;

                if (isValueType)
                {
                    if (!byValueVisualMap.TryGetValue(index, out icc)) byValueVisualMap[index] = (icc = mapper(item, index));
                }
                else
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out icc)) byReferenceVisualMap[item] = (icc = mapper(item, index));
                }

                icc.Index = index;
                icc.DataSource = item;
                index++;

                yield return icc;
            }
        }

        private void OnValuesElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_currentBounds != null && implementsICC)
            {
                var icc = (ICartesianCoordinate)sender;
                // if any limit was modified, then we clear the limits, that means they will be calculate again.
                if (_currentBounds.XCoordinatesBounds.Contains(icc) || _currentBounds.YCoordinatesBounds.Contains(icc))
                    _currentBounds = null;
            }
            NotifySubscribers();
        }

        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_currentBounds != null)
            {
                if (implementsICC)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Move:
                            /// ignored.
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            _currentBounds = null;
                            break;
                    }
                }
            }
            NotifySubscribers();
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
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

        public abstract int GetStackGroup();
    }
}
