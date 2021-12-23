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

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a stacked bar series.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="IChartSeries{TDrawingContext}" />
public interface IStackedBarSeries<TDrawingContext> : IChartSeries<TDrawingContext>, IStrokedAndFilled<TDrawingContext>, ICartesianSeries<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the rx, the radius used in the x axis to round the corners of each column, it goes from 0 to 1.
    /// </summary>
    /// <value>
    /// The rx.
    /// </value>
    double Rx { get; set; }

    /// <summary>
    /// Gets or sets the ry, the radius used in the y axis to round the corners of each column, it goes from 0 to 1.
    /// </summary>
    /// <value>
    /// The ry.
    /// </value>
    double Ry { get; set; }

    /// <summary>
    /// Gets or sets the padding for each group of bars that share the same secondary coordinate.
    /// </summary>
    /// <value>
    /// The bar group padding.
    /// </value>
    double GroupPadding { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the bar.
    /// </summary>
    /// <value>
    /// The maximum width of the bar.
    /// </value>
    double MaxBarWidth { get; set; }

    /// <summary>
    /// Gets or sets the stack group.
    /// </summary>
    /// <value>
    /// The stack group.
    /// </value>
    int StackGroup { get; set; }
}
