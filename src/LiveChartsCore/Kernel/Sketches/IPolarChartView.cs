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

using System.Collections.Generic;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a polar chart view, this view is able to host one or many series in a polar coordinate system.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="IPolarChartView{TDrawingContext}" />
public interface IPolarChartView<TDrawingContext> : IChartView<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets the core.
    /// </summary>
    /// <value>
    /// The core.
    /// </value>
    PolarChart<TDrawingContext> Core { get; }

    /// <summary>
    /// Gets whether the chart scales to try to fit the plot to the series bounds, it calculates a new center of the radial chart,
    /// default is false.
    /// </summary>
    bool FitToBounds { get; set; }

    /// <summary>
    /// Gets or sets the total circumference angle in degrees, default is 360.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    double TotalAngle { get; set; }

    /// <summary>
    /// Gets or sets the inner radius, default is 0.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    double InnerRadius { get; set; }

    /// <summary>
    /// Gets or sets the initial rotation, default is 0.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    double InitialRotation { get; set; }

    /// <summary>
    /// Gets or sets the angle axes.
    /// </summary>
    /// <value>
    /// The angle axes.
    /// </value>
    IEnumerable<IPolarAxis> AngleAxes { get; set; }

    /// <summary>
    /// Gets or sets the radius axes.
    /// </summary>
    /// <value>
    /// The radius axes.
    /// </value>
    IEnumerable<IPolarAxis> RadiusAxes { get; set; }

    /// <summary>
    /// Gets or sets the series to plot in the user interface.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    IEnumerable<ISeries> Series { get; set; }

    /// <summary>
    /// Scales a point in pixels to the chart data scale.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="angleAxisIndex">Index of the angle axis.</param>
    /// <param name="radiusAxisIndex">Index of the radius axis.</param>
    /// <returns></returns>
    LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0);

    /// <summary>
    /// Scales a point in the chart data scale to pixels.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="angleAxisIndex">Index of the x axis.</param>
    /// <param name="radiusAxisIndex">Index of the radius axis.</param>
    /// <returns></returns>
    LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0);
}
