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

using LiveChartsCore.Drawing;
using System;
using System.Drawing;
using LiveChartsCore.Measure;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    public interface IAxis
    {
        /// <summary>
        /// Gets the previous data bounds.
        /// </summary>
        /// <value>
        /// The previous data bounds.
        /// </value>
        Bounds? PreviousDataBounds { get; }

        /// <summary>
        /// Gets the data bounds, the min and max values in the axis.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds DataBounds { get; }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// Gets or sets the xo, a refence used internally to calculate the axis position.
        /// </summary>
        /// <value>
        /// The xo.
        /// </value>
        float Xo { get; set; }

        /// <summary>
        /// Gets or sets the yo, a refence used internally to calculate the axis position..
        /// </summary>
        /// <value>
        /// The yo.
        /// </value>
        float Yo { get; set; }

        /// <summary>
        /// Gets or sets the labeler, a function that receives a number and return the label content as string.
        /// </summary>
        /// <value>
        /// The labeler.
        /// </value>
        Func<double, AxisTick, string> Labeler { get; set; }

        /// <summary>
        /// Gets or sets the step, the step defines the interval between every separator in the axis, when the step is null, 
        /// LiveCharts will calculate it automatically based on the chart data and size, default is null (automatic).
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        double? Step { get; set; }

        /// <summary>
        /// Gets or sets the unit with, it means the width of every point (if the series requires it) in the chart values scale, this value
        /// should normally be 1, unless you are plotting in a custom scale, default is 1.
        /// </summary>
        /// <value>
        /// The unit with.
        /// </value>
        double UnitWith { get; set; }

        /// <summary>
        /// Gets or sets the minimum value visible in the axis, any point less than this value will be hidden, 
        /// set it to null to use a value based on the smaller value in the chart.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        double? MinLimit { get; set; }

        /// <summary>
        /// Gets or sets the maximum value visible in the axis, any point greater than this value will be hidden, 
        /// set it null to use a value based on the greather value in the chart.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        double? MaxLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis is inverted based on the Cartesian coordinate system.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inverted; otherwise, <c>false</c>.
        /// </value>
        bool IsInverted { get; set; }

        AxisPosition Position { get; set; }
        double LabelsRotation { get; set; }

        bool ShowSeparatorLines { get; set; }
        bool ShowSeparatorWedges { get; set; }

        void Initialize(AxisOrientation orientation);
    }

    public interface IAxis<TDrawingContext> : IAxis
        where TDrawingContext : DrawingContext
    {
        IDrawableTask<TDrawingContext>? TextBrush { get; set; }

        IDrawableTask<TDrawingContext>? SeparatorsBrush { get; set; }

        IDrawableTask<TDrawingContext>? AlternativeSeparatorForeground { get; set; }

        void Measure(CartesianChart<TDrawingContext> chart);

        SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart);

        /// <summary>
        /// Gets the deleting tasks.
        /// </summary>
        /// <value>
        /// The deleting tasks.
        /// </value>
        List<IDrawableTask<TDrawingContext>> DeletingTasks { get; }
    }
}