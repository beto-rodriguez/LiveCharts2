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
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an object that must initialize live charts visual objects, this object defines how things will 
    /// be drawn by default, it is highly related to themes.
    /// </summary>
    public abstract class LiveChartsInitializer<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Constructs a chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public abstract void ConstructChart(IChartView<TDrawingContext> chart);


        /// <summary>
        /// Constructs a series.
        /// </summary>
        /// <param name="series">The series.</param>
        public abstract void ConstructSeries(IDrawableSeries<TDrawingContext> series);


        /// <summary>
        /// Constructs an axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public abstract void ConstructAxis(IAxis<TDrawingContext> axis);

        /// <summary>
        /// Resolves the series defaults.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="series">The series.</param>
        public abstract void ResolveSeriesDefaults(Color[] colors, IDrawableSeries<TDrawingContext> series);

        /// <summary>
        /// Resolves the axis defaults.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public abstract void ResolveAxisDefaults(IAxis<TDrawingContext> axis);
    }
}
