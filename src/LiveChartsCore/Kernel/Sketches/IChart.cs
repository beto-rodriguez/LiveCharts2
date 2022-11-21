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

using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a chart.
/// </summary>
public interface IChart
{
    /// <summary>
    /// Gets or sets the measure work.
    /// </summary>
    /// <value>
    /// The measure work.
    /// </value>
    object MeasureWork { get; }

    /// <summary>
    /// Gets the chart view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    IChartView View { get; }

    /// <summary>
    /// Gets the canvas.
    /// </summary>
    /// <value>
    /// The canvas.
    /// </value>
    object Canvas { get; }

    /// <summary>
    /// Gets the legend position.
    /// </summary>
    /// <value>
    /// The legend position.
    /// </value>
    LegendPosition LegendPosition { get; }

    /// <summary>
    /// Gets the toolTip position.
    /// </summary>
    /// <value>
    /// The toolTip position.
    /// </value>
    TooltipPosition TooltipPosition { get; }

    /// <summary>
    /// Gets the toolTip finding strategy.
    /// </summary>
    /// <value>
    /// The toolTip finding strategy.
    /// </value>
    TooltipFindingStrategy TooltipFindingStrategy { get; }

    /// <summary>
    /// Updates the chart in the user interface.
    /// </summary>
    void Update(ChartUpdateParams? chartUpdateParams = null);
}
