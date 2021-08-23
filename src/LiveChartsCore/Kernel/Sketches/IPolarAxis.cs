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
using System.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Sketches
{
    /// <summary>
    /// Defines a polar axis.
    /// </summary>
    public interface IPolarAxis : IPlane
    {
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        PolarAxisOrientation Orientation { get; }

        /// <summary>
        /// Initializes the axis for the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        void Initialize(PolarAxisOrientation orientation);

        /// <summary>
        /// Occurs when the axis is initialized.
        /// </summary>
        event Action<IPolarAxis>? Initialized;
    }

    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IDisposable" />
    public interface IPolarAxis<TDrawingContext> : IPolarAxis, IChartElement<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the name paint.
        /// </summary>
        /// <value>
        /// The text paint.
        /// </value>
        IPaint<TDrawingContext>? NamePaint { get; set; }

        /// <summary>
        /// Gets or sets the text paint.
        /// </summary>
        /// <value>
        /// The text paint.
        /// </value>
        IPaint<TDrawingContext>? LabelsPaint { get; set; }

        /// <summary>
        /// Gets or sets the separators paint.
        /// </summary>
        /// <value>
        /// The separators paint.
        /// </value>
        IPaint<TDrawingContext>? SeparatorsPaint { get; set; }

        /// <summary>
        /// Gets the size of the possible.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart);

        /// <summary>
        /// Gets the size of the axis name label.
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        SizeF GetNameLabelSize(CartesianChart<TDrawingContext> chart);
    }
}
