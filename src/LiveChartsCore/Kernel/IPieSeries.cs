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

using LiveChartsCore.Drawing;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a pie series.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="LiveChartsCore.Kernel.IDrawableSeries{TDrawingContext}" />
    public interface IPieSeries<TDrawingContext>: IDrawableSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the pushout, it is the distance in pixels between the center of the control and the pie slice.
        /// </summary>
        /// <value>
        /// The pushout.
        /// </value>
        double Pushout { get; set; }

        /// <summary>
        /// Gets or sets the inner radius of the slice in pixels.
        /// </summary>
        /// <value>
        /// The inner radius.
        /// </value>
        double InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the maximum outer, the value goes from 0 to 1, where 1 is the full available radius and 0 is none.
        /// </summary>
        /// <value>
        /// The maximum outer radius.
        /// </value>
        double MaxOuterRadius { get; set; }

        /// <summary>
        /// Gets or sets the hover pushout in pixes, it defines the <see cref="Pushout"/> where the pointer is over the slice.
        /// </summary>
        /// <value>
        /// The hover pushout.
        /// </value>
        double HoverPushout { get; set; }

        /// <summary>
        /// Gets the series bounds.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        DimensinalBounds GetBounds(PieChart<TDrawingContext> chart);

        /// <summary>
        /// Measures the series and schedules the draw in specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        void Measure(PieChart<TDrawingContext> chart);
    }
}
