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
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines the environment of a point in a chart.
/// </summary>
public class ChartPointContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPointContext"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="entity">The entity.</param>
    public ChartPointContext(IChartView chart, ISeries series, IChartEntity entity)
    {
        Chart = chart;
        Series = series;
        Entity = entity;
    }

    internal ChartPointContext()
    {
        // dummy empty constructor..
        // This is used only when the IChartEntity was null
        Chart = null!;
        Series = null!;
        Entity = new MappedChartEntity();
    }

    /// <summary>
    /// Gets the chart.
    /// </summary>
    /// <value>
    /// The chart.
    /// </value>
    public IChartView Chart { get; }

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public ISeries Series { get; }

    /// <summary>
    /// Gets the <see cref="IChartEntity"/>.
    /// </summary>
    public IChartEntity Entity { get; }

    /// <summary>
    /// Gets the DataSource.
    /// </summary>
    public object? DataSource { get; internal set; }

    /// <summary>
    /// Gets the position of the point the collection that was used when the point was drawn.
    /// </summary>
    [Obsolete($"use {nameof(ChartPoint)}.{nameof(ChartPoint.Index)} instead")]
    public int Index => Entity?.MetaData?.EntityIndex ?? 0;

    /// <summary>
    /// Gets the visual.
    /// </summary>
    /// <value>
    /// The visual.
    /// </value>
    public object? Visual { get; internal set; }

    /// <summary>
    /// Gets the additional visuals, some series might have more than one visual, here you can find the rest of them.
    /// </summary>
    public object? AdditionalVisuals { get; internal set; }

    /// <summary>
    /// Gets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    public object? Label { get; internal set; }

    /// <summary>
    /// Gets or sets the area that triggers the ToolTip.
    /// </summary>
    public HoverArea? HoverArea { get; internal set; }
}
