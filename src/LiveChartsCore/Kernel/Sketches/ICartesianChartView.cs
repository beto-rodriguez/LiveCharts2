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
using LiveChartsCore.Measure;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Kernel.Sketches
{
    /// <summary>
    /// Defines a Cartesian chart view, this view is able to host one or many series in a Cartesian coordinate system.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IChartView{TDrawingContext}" />
    public interface ICartesianChartView<TDrawingContext> : IChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the core.
        /// </summary>
        /// <value>
        /// The core.
        /// </value>
        CartesianChart<TDrawingContext> Core { get; }

        /// <summary>
        /// Gets or sets the x axes.
        /// </summary>
        /// <value>
        /// The x axes.
        /// </value>
        IEnumerable<IAxis> XAxes { get; set; }

        /// <summary>
        /// Gets or sets the y axes.
        /// </summary>
        /// <value>
        /// The y axes.
        /// </value>
        IEnumerable<IAxis> YAxes { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        IEnumerable<Section<TDrawingContext>> Sections { get; set; }

        /// <summary>
        /// Gets or sets the series to plot in the user interface.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        IEnumerable<ISeries> Series { get; set; }

        /// <summary>
        /// Gets or sets the draw margin frame.
        /// </summary>
        /// <value>
        /// The draw margin frame.
        /// </value>
        DrawMarginFrame<TDrawingContext>? DrawMarginFrame { get; set; }

        /// <summary>
        /// Gets or sets the zoom mode.
        /// </summary>
        /// <value>
        /// The zoom mode.
        /// </value>
        ZoomAndPanMode ZoomMode { get; set; }


        /// <summary>
        /// Gets or sets the tool tip finding strategy.
        /// </summary>
        /// <value>
        /// The tool tip finding strategy.
        /// </value>
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <summary>
        /// Gets or sets the zooming speed from 0 to 1, where 0 is the fastest and 1 the slowest.
        /// </summary>
        /// <value>
        /// The zooming speed.
        /// </value>
        double ZoomingSpeed { get; set; }

        /// <summary>
        /// Scales the UI point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="xAxisIndex">Index of the x axis.</param>
        /// <param name="yAxisIndex">Index of the y axis.</param>
        /// <returns></returns>
        double[] ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0);
    }
}
