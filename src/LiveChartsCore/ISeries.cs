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

using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a chart series.
    /// </summary>
    public interface ISeries
    {
        /// <summary>
        /// Gets or sets a series unique identifier, the library handles this id automatically.
        /// </summary>
        int SeriesId { get; set; }

        /// <summary>
        /// Gets or sets the state where the visual is moved to when the mouse moves over a <see cref="ChartPoint"/>.
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
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the data padding, both coordinates (X and Y) from 0 to 1, where 0 is nothing and 1 is the axis tick
        /// (the separation between every label).
        /// </summary>
        /// <value>
        /// The data padding.
        /// </value>
        PointF DataPadding { get; set; }

        /// <summary>
        /// Gets or sets the z index position.
        /// </summary>
        /// <value>
        /// The index of the z.
        /// </value>
        int ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the value where the direction of the axis changes, by default is 0.
        /// </summary>
        double Pivot { get; set; }

        /// <summary>
        /// Gets or sets the animations speed, if this property is null, the
        /// <see cref="Chart{TDrawingContext}.AnimationsSpeed"/> property will be used.
        /// </summary>
        /// <value>
        /// The animations speed.
        /// </value>
        TimeSpan? AnimationsSpeed { get; set; }

        /// <summary>
        /// Gets or sets the easing function to animate the series, if this property is null, the
        /// <see cref="Chart{TDrawingContext}.EasingFunction"/> property will be used.
        /// </summary>
        /// <value>
        /// The easing function.
        /// </value>
        Func<float, float>? EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is notifying changes, this property is used internally to turn off
        /// notifications while the theme is being applied, this property is not designed to be used by the user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is notifying changes; otherwise, <c>false</c>.
        /// </value>
        bool IsNotifyingChanges { get; set; }

        /// <summary>
        /// Gets the tooltip text for a give chart point.
        /// </summary>
        /// <param name="point">The chart point.</param>
        /// <returns></returns>
        string GetTooltipText(ChartPoint point);

        /// <summary>
        /// Gets the data label content for a given chart point.
        /// </summary>
        /// <param name="point">The chart point.</param>
        /// <returns></returns>
        string GetDataLabelText(ChartPoint point);

        /// <summary>
        /// Gets a <see cref="ChartPoint"/> array with the points used to generate the plot.
        /// </summary>
        /// <param name="chart">the chart</param>
        /// <returns></returns>
        IEnumerable<ChartPoint> Fetch(IChart chart);

        /// <summary>
        /// Gets the <see cref="ChartPoint"/> instances which contain the <paramref name="pointerPosition"/>, according 
        /// to the chart's <see cref="TooltipFindingStrategy"/> property.
        /// </summary>
        /// <param name="chart">the chart</param>
        /// <param name="pointerPosition">the pointer position</param>
        /// <param name="automaticStategy">the already resolved strategy when strategy is set to automatic.</param>
        /// <returns></returns>
        IEnumerable<TooltipPoint> FindPointsNearTo(IChart chart, PointF pointerPosition, TooltipFindingStrategy automaticStategy);

        /// <summary>
        /// Marks a given point as a given state.
        /// </summary>
        /// <param name="chartPoint"></param>
        /// <param name="state"></param>
        void AddPointToState(ChartPoint chartPoint, string state);

        /// <summary>
        /// Removes a given point from the given state.
        /// </summary>
        /// <param name="chartPoint"></param>
        /// <param name="state"></param>
        void RemovePointFromState(ChartPoint chartPoint, string state);

        /// <summary>
        /// Clears the visuals in the cache and re-starts animations.
        /// </summary>
        void RestartAnimations();

        /// <summary>
        /// Deletes the series from the user interface.
        /// </summary>
        void SoftDelete(IChartView chart);
    }

    /// <summary>
    /// Defines a series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <seealso cref="IDisposable" />
    public interface ISeries<TModel> : ISeries
    {
        /// <summary>
        /// Gets or sets the mapping.
        /// </summary>
        /// <value>
        /// The mapping.
        /// </value>
        Action<TModel, ChartPoint>? Mapping { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        new IEnumerable<TModel>? Values { get; set; }
    }
}
