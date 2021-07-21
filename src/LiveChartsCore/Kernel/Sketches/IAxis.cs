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

using LiveChartsCore.Drawing;
using System;
using System.Drawing;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using LiveChartsCore.Drawing.Common;
using System.ComponentModel;

namespace LiveChartsCore.Kernel.Sketches
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart. 
    /// </summary>
    public interface IAxis : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the axis name.
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// Gets or sets the size of the name label.
        /// </summary>
        /// <value>
        /// The size of the text.
        /// </value>
        double NameTextSize { get; set; }

        /// <summary>
        /// Gets or sets the padding of the name label.
        /// </summary>
        Padding NamePadding { get; set; }

        /// <summary>
        /// Gets the previous data bounds.
        /// </summary>
        /// <value>
        /// The previous data bounds.
        /// </value>
        Bounds? PreviousDataBounds { get; set; }

        /// <summary>
        /// Gets the previous data bounds.
        /// </summary>
        /// <value>
        /// The previous data bounds.
        /// </value>
        Bounds? PreviousVisibleDataBounds { get; set; }

        /// <summary>
        /// Gets the data bounds, the min and max values in the axis.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds DataBounds { get; }

        /// <summary>
        /// Gets the data visible bounds, the min and max visible values in the axis.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds VisibleDataBounds { get; }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the xo, a reference used internally to calculate the axis position.
        /// </summary>
        /// <value>
        /// The xo.
        /// </value>
        float Xo { get; set; }

        /// <summary>
        /// Gets or sets the yo, a reference used internally to calculate the axis position..
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
        Func<double, string> Labeler { get; set; }

        /// <summary>
        /// Gets or sets the minimum step, the step defines the interval between every separator in the axis,
        /// LiveCharts will calculate it automatically based on the chart data and the chart size size, if the calculated step is less than the <see cref="MinStep"/> 
        /// then <see cref="MinStep"/> will be used as the axis step, default is 0.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        double MinStep { get; set; }

        /// <summary>
        /// Gets or sets the unit with, it means the width of every point (if the series requires it) in the chart values scale, this value
        /// should normally be 1, unless you are plotting in a custom scale, default is 1.
        /// </summary>
        /// <value>
        /// The unit with.
        /// </value>
        double UnitWidth { get; set; }

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
        /// set it null to use a value based on the greater value in the chart.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        double? MaxLimit { get; set; }

        /// <summary>
        /// Gets or sets the previous maximum limit.
        /// </summary>
        /// <value>
        /// The previous maximum limit.
        /// </value>
        double? PreviousMaxLimit { get; set; }

        /// <summary>
        /// Gets or sets the previous minimum limit.
        /// </summary>
        /// <value>
        /// The previous minimum limit.
        /// </value>
        double? PreviousMinLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis is inverted based on the Cartesian coordinate system.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inverted; otherwise, <c>false</c>.
        /// </value>
        bool IsInverted { get; set; }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        AxisPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the labels rotation in degrees.
        /// </summary>
        /// <value>
        /// The labels rotation.
        /// </value>
        double LabelsRotation { get; set; }

        /// <summary>
        /// Gets or sets the size of the labels.
        /// </summary>
        /// <value>
        /// The size of the text.
        /// </value>
        double TextSize { get; set; }

        /// <summary>
        /// Gets or sets the labels, if labels are not null, then the axis label will be pulled from the labels collection,
        /// the label is mapped to the chart based on the position of the label and the position of the point, both integers,
        /// if the axis requires a label outside the bounds of the labels collection, then the index will be returned as the label.
        /// Default value is null.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        IList<string>? Labels { get; set; }

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
        /// Gets or sets a value indicating whether the separator lines are visible.
        /// </summary>
        bool ShowSeparatorLines { get; set; }

        /// <summary>
        /// Initializes the axis for the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        void Initialize(AxisOrientation orientation);

        /// <summary>
        /// Occurs when the axis is initialized.
        /// </summary>
        event Action<IAxis>? Initialized;
    }

    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IDisposable" />
    public interface IAxis<TDrawingContext> : IAxis, IChartElement<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the name paint.
        /// </summary>
        /// <value>
        /// The text paint.
        /// </value>
        IPaintTask<TDrawingContext>? NamePaint { get; set; }

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
        [Obsolete("Renamed to TextPaint")]
        IPaintTask<TDrawingContext>? TextBrush { get; set; }

        /// <summary>
        /// Gets or sets the text paint.
        /// </summary>
        /// <value>
        /// The text paint.
        /// </value>
        IPaintTask<TDrawingContext>? LabelsPaint { get; set; }

        /// <summary>
        /// Gets or sets the separators brush.
        /// </summary>
        /// <value>
        /// The separators brush.
        /// </value>
        [Obsolete("Renamed to SeparatorsPaint")]
        IPaintTask<TDrawingContext>? SeparatorsBrush { get; set; }

        /// <summary>
        /// Gets or sets the separators paint.
        /// </summary>
        /// <value>
        /// The separators paint.
        /// </value>
        IPaintTask<TDrawingContext>? SeparatorsPaint { get; set; }

        /// <summary>
        /// Gets the size of the possible.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart);

        /// <summary>
        /// Gets the size of the axis name label.
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        SizeF GetNameLabelSize(CartesianChart<TDrawingContext> chart);
    }
}
