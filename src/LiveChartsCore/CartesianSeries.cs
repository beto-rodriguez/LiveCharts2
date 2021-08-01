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
using LiveChartsCore.Measure;
using System.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Data;

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

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public virtual SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            return dataProvider is null
                ? throw new Exception("A data provider is required")
                : dataProvider.GetCartesianBounds(chart, this, x, y);
        }

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
                DataLabelsPosition.Middle => new PointF(middleX, middleY),
                DataLabelsPosition.Top => new PointF(middleX, y - labelSize.Height * 0.5f),
                DataLabelsPosition.Bottom => new PointF(middleX, y + height + labelSize.Height * 0.5f),
                DataLabelsPosition.Left => new PointF(x - labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.Right => new PointF(x + width + labelSize.Width * 0.5f, middleY),
                DataLabelsPosition.End =>
                (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                    ? (isGreaterThanPivot
                        ? new PointF(x + width + labelSize.Width * 0.5f, middleY)
                        : new PointF(x - labelSize.Width * 0.5f, middleY))
                    : (isGreaterThanPivot
                        ? new PointF(middleX, y - labelSize.Height * 0.5f)
                        : new PointF(middleX, y + height + labelSize.Height * 0.5f)),
                DataLabelsPosition.Start =>
                     (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                        ? (isGreaterThanPivot
                            ? new PointF(x - labelSize.Width * 0.5f, middleY)
                            : new PointF(x + width + labelSize.Width * 0.5f, middleY))
                        : (isGreaterThanPivot
                            ? new PointF(middleX, y + height + labelSize.Height * 0.5f)
                            : new PointF(middleX, y - labelSize.Height * 0.5f)),
                _ => throw new Exception("Position not supported"),
            };
        }
    }
}
