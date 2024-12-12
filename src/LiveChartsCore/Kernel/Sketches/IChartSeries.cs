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
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a series a chart series that has a visual representation in the user interface.
/// </summary>
/// <seealso cref="ISeries" />
public interface IChartSeries : ISeries, IChartElement
{
    /// <summary>
    /// Gets or sets the data labels paint.
    /// </summary>
    /// <value>
    /// The data labels paint.
    /// </value>
    Paint? DataLabelsPaint { get; set; }

    /// <summary>
    /// Gets or sets the size of the data labels.
    /// </summary>
    /// <value>
    /// The size of the data labels.
    /// </value>
    double DataLabelsSize { get; set; }

    /// <summary>
    /// Gets or sets the data labels rotation in degrees.
    /// </summary>
    /// <value>
    /// The rotation of the data labels in degrees.
    /// </value>
    double DataLabelsRotation { get; set; }

    /// <summary>
    /// Gets or sets the data labels padding.
    /// </summary>
    /// <value>
    /// The data labels padding.
    /// </value>
    Padding DataLabelsPadding { get; set; }

    /// <summary>
    /// Gets or sets the max width of the data labels.
    /// </summary>
    /// <value>
    /// The max with of the data labels.
    /// </value>
    double DataLabelsMaxWidth { get; set; }

    /// <summary>
    /// Gets the stack group, normally used internally to handled the stacked series.
    /// </summary>
    /// <returns></returns>
    int GetStackGroup();

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [Obsolete($"Replaced by ${nameof(GetMiniatureGeometry)}")]
    Sketch GetMiniaturesSketch();

    /// <summary>
    /// Return the visual element shown in tooltips and legends, this is an old method and will be replaced by
    /// <see cref="GetMiniatureGeometry(ChartPoint?)"/>.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="zindex">The zindex.</param>
    [Obsolete($"Replaced by ${nameof(GetMiniatureGeometry)}")]
    IChartElement GetMiniature(ChartPoint? point, int zindex);

    /// <summary>
    /// Returns a geometry that represents the series in a tooltip or legend.
    /// </summary>
    /// <param name="point">The target point.</param>
    IDrawnElement GetMiniatureGeometry(ChartPoint? point);

    /// <summary>
    /// Called when the pointer goes down on a data point or points.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="points">The found points.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    void OnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points, LvcPoint pointerLocation);
}
