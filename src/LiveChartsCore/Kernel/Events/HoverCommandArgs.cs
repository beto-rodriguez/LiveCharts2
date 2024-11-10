
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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Command arguments that defines the changes in the hovered points in a chart.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HoverCommandArgs"/> class.
/// </remarks>
/// <param name="chart">The chart that fired the event.</param>
/// <param name="newPoints">The new points.</param>
/// <param name="oldPoints">The old points.</param>
public class HoverCommandArgs(
    IChartView chart,
    IEnumerable<ChartPoint>? newPoints,
    IEnumerable<ChartPoint>? oldPoints)
        : ChartCommandArgs(chart)
{
    /// <summary>
    /// Gets the new hovered points in the chart.
    /// </summary>
    public IEnumerable<ChartPoint>? NewPoints { get; set; } = newPoints;

    /// <summary>
    /// Gets the points that are no longer hovered in the chart.
    /// </summary>
    public IEnumerable<ChartPoint>? OldPoints { get; } = oldPoints;
}
