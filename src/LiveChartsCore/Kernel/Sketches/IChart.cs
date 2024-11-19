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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

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
    CoreMotionCanvas Canvas { get; }

    /// <summary>
    /// Gets the series context.
    /// </summary>
    SeriesContext SeriesContext { get; }

    /// <summary>
    /// Gets the control size.
    /// </summary>
    LvcSize ControlSize { get; }

    /// <summary>
    /// Gets the draw margin location.
    /// </summary>
    LvcPoint DrawMarginLocation { get; }

    /// <summary>
    /// Gets the draw margin size.
    /// </summary>
    LvcSize DrawMarginSize { get; }

    /// <summary>
    /// Gets the easing function.
    /// </summary>
    Func<float, float>? EasingFunction { get; }

    /// <summary>
    /// Gets the animations speed.
    /// </summary>
    TimeSpan AnimationsSpeed { get; }

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
    /// Gets the finding strategy.
    /// </summary>
    /// <value>
    /// The finding strategy.
    /// </value>
    FindingStrategy FindingStrategy { get; }

    /// <summary>
    /// Gets whether the series has been drawn in the user interface.
    /// </summary>
    /// <param name="seriesId">The series id.</param>
    /// <returns>A boolean that indicates whether the series has been drawn.</returns>
    bool IsDrawn(int seriesId);

    /// <summary>
    /// Updates the chart in the user interface.
    /// </summary>
    void Update(ChartUpdateParams? chartUpdateParams = null);

    /// <summary>
    /// Adds a visual element to the chart.
    /// </summary>
    /// <param name="element"></param>
    void AddVisual(IChartElement element);

    /// <summary>
    /// Removes a visual element from the chart.
    /// </summary>
    /// <param name="element"></param>
    void RemoveVisual(IChartElement element);
}
