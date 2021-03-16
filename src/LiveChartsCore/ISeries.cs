// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a chart series.
    /// </summary>
    public interface ISeries : IDisposable
    {
        /// <summary>
        /// Gets or sets a series unique identifier, the library handles this id automatically.
        /// </summary>
        int SeriesId { get; set; }

        /// <summary>
        /// Gets or sets the state where the visual is moved to when the mouse moves over a <see cref="ChartPoint{TModel, TVisual, TLabel, TDrawingContext}"/>.
        /// </summary>
        string HoverState { get; set; }

        /// <summary>
        /// Gets the properties of the series.
        /// </summary>
        SeriesProperties SeriesProperties { get; }

        /// <summary>
        /// Gets or sets the name of the series, the name is normally used by <see cref="IChartTooltip{TDrawingContext}"/> or 
        /// <see cref="IChartLegend{TDrawingContext}"/>, the default value is set automatically by the library.
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        IEnumerable? Values { get; set; }

        /// <summary>
        /// Gets or sets the value where the direction of the axis changes, by default is 0.
        /// </summary>
        double Pivot { get; set; }

        /// <summary>
        /// Gets a <see cref="IChartPoint"/> array with the points used to generate the plot.
        /// </summary>
        /// <param name="chart">the chart</param>
        /// <returns></returns>
        IChartPoint[] Fetch(IChart chart);

        /// <summary>
        /// Gets the <see cref="IChartPoint"/> instances which contain the <paramref name="pointerPosition"/>, according 
        /// to the chart's <see cref="TooltipFindingStrategy"/> property.
        /// </summary>
        /// <param name="chart">the chart</param>
        /// <param name="pointerPosition">the poiinter position</param>
        /// <returns></returns>
        IEnumerable<TooltipPoint> FindPointsNearTo(IChart chart, PointF pointerPosition);

        /// <summary>
        /// Marks a given point as a given state.
        /// </summary>
        /// <param name="chartPoint"></param>
        /// <param name="state"></param>
        void AddPointToState(IChartPoint chartPoint, string state);

        /// <summary>
        /// Removes a given point from the given state.
        /// </summary>
        /// <param name="chartPoint"></param>
        /// <param name="state"></param>
        void RemovePointFromState(IChartPoint chartPoint, string state);
    }
}
