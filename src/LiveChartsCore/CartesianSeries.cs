﻿// The MIT License(MIT)

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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines data to plot in a chart.
    /// </summary>
    public abstract class CartesianSeries<TModel, TVisual, TDrawingContext>
        : DrawableSeries<TModel, TVisual, TDrawingContext>, IDisposable, ICartesianSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Series{T}"/> class.
        /// </summary>
        public CartesianSeries(SeriesProperties properties): base(properties) { }

        /// <inheritdoc/>
        public int ScalesXAt { get; set; }

        /// <inheritdoc/>
        public int ScalesYAt { get; set; }

        /// <inheritdoc/>
        public virtual DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var stack = chart.SeriesContext.GetStackPosition(this, GetStackGroup());

            var bounds = new DimensinalBounds();
            foreach (var point in Fetch(chart))
            {
                var primary = point.PrimaryValue;
                var secondary = point.SecondaryValue;

                // it has more sense to override this method and call the stack, only if the series requires so.
                if (stack != null) primary = stack.StackPoint(point);

                bounds.PrimaryBounds.AppendValue(primary);
                bounds.SecondaryBounds.AppendValue(secondary);
            }

            return bounds;
        }

        /// <inheritdoc/>
        public abstract void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);
    }
}
