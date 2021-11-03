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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a Cartesian series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ICartesianSeries{TDrawingContext}" />
    public abstract class CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>
        : ChartSeries<TModel, TVisual, TLabel, TDrawingContext>, ICartesianSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private int _scalesXAt;
        private int _scalesYAt;
        private DataLabelsPosition _labelsPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The series properties.</param>
        protected CartesianSeries(SeriesProperties properties) : base(properties) { }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesXAt"/>
        public int ScalesXAt { get => _scalesXAt; set { _scalesXAt = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesYAt"/>
        public int ScalesYAt { get => _scalesYAt; set { _scalesYAt = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.DataLabelsPosition"/>
        public DataLabelsPosition DataLabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
        public virtual SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, ICartesianAxis x, ICartesianAxis y)
        {
            return DataFactory is null
                ? throw new Exception("A data provider is required")
                : DataFactory.GetCartesianBounds(chart, this, x, y);
        }

        /// <summary>
        /// Deletes the series from the user interface.
        /// </summary>
        /// <param name="chart"></param>
        /// <inheritdoc cref="ISeries.SoftDeleteOrDispose(IChartView)" />
        public override void SoftDeleteOrDispose(IChartView chart)
        {
            var core = ((ICartesianChartView<TDrawingContext>)chart).Core;

            var secondaryAxis = core.XAxes[ScalesXAt];
            var primaryAxis = core.YAxes[ScalesYAt];

            var secondaryScale = new Scaler(core.DrawMarginLocation, core.DrawMarginSize, secondaryAxis);
            var primaryScale = new Scaler(core.DrawMarginLocation, core.DrawMarginSize, primaryAxis);

            var deleted = new List<ChartPoint>();
            foreach (var point in everFetched)
            {
                if (point.Context.Chart != chart) continue;

                SoftDeleteOrDisposePoint(point, primaryScale, secondaryScale);
                deleted.Add(point);
            }

            foreach (var pt in GetPaintTasks())
            {
                if (pt is not null) core.Canvas.RemovePaintTask(pt);
            }

            foreach (var item in deleted) _ = everFetched.Remove(item);

            OnVisibilityChanged();
        }

        /// <summary>
        /// Softs the delete point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="primaryScale">The primary scale.</param>
        /// <param name="secondaryScale">The secondary scale.</param>
        /// <returns></returns>
        protected internal abstract void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale);

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
        /// <param name="drawMarginLocation">The draw margin location.</param>
        /// <param name="drawMarginSize">the draw margin size</param>
        /// <returns></returns>
        protected internal virtual LvcPoint GetLabelPosition(
            float x,
            float y,
            float width,
            float height,
            LvcSize labelSize,
            DataLabelsPosition position,
            SeriesProperties seriesProperties,
            bool isGreaterThanPivot,
            LvcPoint drawMarginLocation,
            LvcSize drawMarginSize)
        {
            if ((seriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
            {
                var oy = y + height;
                if (y < drawMarginLocation.Y) y = drawMarginLocation.Y;
                var maxHeight = isGreaterThanPivot
                    ? drawMarginLocation.Y + drawMarginSize.Height - y
                    : oy - y;
                if (height > maxHeight) height = maxHeight;

                var ox = x + width;
                if (x < drawMarginLocation.X) x = drawMarginLocation.X;
                var maxWidth = isGreaterThanPivot
                    ? drawMarginLocation.X + drawMarginSize.Width - x
                    : ox - x;
                if (width > maxWidth) width = maxWidth;
            }

            var middleX = (x + x + width) * 0.5f;
            var middleY = (y + y + height) * 0.5f;

            return position switch
            {
                DataLabelsPosition.Middle
                    => new LvcPoint(middleX, middleY),
                DataLabelsPosition.Top
                    => new LvcPoint(middleX, y - labelSize.Height * 0.5f),
                DataLabelsPosition.Bottom
                    => new LvcPoint(middleX, y + height + labelSize.Height * 0.5f),
                DataLabelsPosition.Left
                    => new LvcPoint(x - labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.Right
                    => new LvcPoint(x + width + labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.End =>
                    (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                        ? (isGreaterThanPivot
                            ? new LvcPoint(x + width + labelSize.Width * 0.5f, middleY)
                            : new LvcPoint(x - labelSize.Width * 0.5f, middleY))
                        : (isGreaterThanPivot
                            ? new LvcPoint(middleX, y - labelSize.Height * 0.5f)
                            : new LvcPoint(middleX, y + height + labelSize.Height * 0.5f)),
                DataLabelsPosition.Start =>
                     (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                        ? (isGreaterThanPivot
                            ? new LvcPoint(x - labelSize.Width * 0.5f, middleY)
                            : new LvcPoint(x + width + labelSize.Width * 0.5f, middleY))
                        : (isGreaterThanPivot
                            ? new LvcPoint(middleX, y + height + labelSize.Height * 0.5f)
                            : new LvcPoint(middleX, y - labelSize.Height * 0.5f)),
                _ => throw new Exception("Position not supported"),
            };
        }
    }
}
