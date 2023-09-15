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
using LiveChartsCore.Kernel.Drawing;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a series a chart series that has a visual representation in the user interface.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="ISeries" />
public interface IChartSeries<TDrawingContext> : ISeries, IChartElement<TDrawingContext>
     where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the data labels paint.
    /// </summary>
    /// <value>
    /// The data labels paint.
    /// </value>
    IPaint<TDrawingContext>? DataLabelsPaint { get; set; }

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
    /// Gets the paint schedule, normally handled internally to display tool tips and legends.
    /// </summary>
    /// <value>
    /// The default paint context.
    /// </value>
    Sketch<TDrawingContext> CanvasSchedule { get; }

    /// <summary>
    /// Gets the stack group, normally used internally to handled the stacked series.
    /// </summary>
    /// <returns></returns>
    int GetStackGroup();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Sketch<TDrawingContext> GetMiniaturesSketch();

    /// <summary>
    /// Called when the pointer goes down on a data point or points.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="points">The found points.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    void OnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points, LvcPoint pointerLocation);
}
