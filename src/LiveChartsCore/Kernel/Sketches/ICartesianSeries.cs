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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Data;
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Sketches
{
    /// <summary>
    /// Defines a Cartesian series.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IChartSeries{TDrawingContext}" />
    public interface ICartesianSeries<TDrawingContext> : IChartSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the axis index where the series is scaled in the X plane, the index must exist 
        /// in the <see cref="ICartesianChartView{TDrawingContext}.XAxes"/> collection.
        /// </summary>
        /// <value>
        /// The index of the axis.
        /// </value>
        int ScalesXAt { get; set; }

        /// <summary>
        /// Gets or sets the axis index where the series is scaled in the Y plane, the index must exist 
        /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
        /// </summary>
        /// <value>
        /// The index of the axis.
        /// </value>
        int ScalesYAt { get; set; }

        /// <summary>
        /// Gets or sets the data labels position.
        /// </summary>
        /// <value>
        /// The data labels position.
        /// </value>
        DataLabelsPosition DataLabelsPosition { get; set; }

        /// <summary>
        /// Gets the series bounds.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>the series bounds</returns>
        SeriesBounds GetBounds(CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);
    }
}
