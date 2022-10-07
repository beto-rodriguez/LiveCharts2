﻿// The MIT License(MIT)
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

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a pie chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="IChartView{TDrawingContext}" />
public interface IPieChartView<TDrawingContext> : IChartView<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets the core.
    /// </summary>
    /// <value>
    /// The core.
    /// </value>
    PieChart<TDrawingContext> Core { get; }

    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    IEnumerable<ISeries> Series { get; set; }

    /// <summary>
    /// Gets or sets the visual elements.
    /// </summary>
    IEnumerable<ChartElement<TDrawingContext>> VisualElements { get; set; }

    /// <summary>
    /// Gets or sets the initial rotation in degrees, this angle specifies where the first pie slice will be drawn, then the remaining
    /// slices will stack according to its corresponding position.
    /// </summary>
    /// <value>
    /// The initial rotation.
    /// </value>
    public double InitialRotation { get; set; }

    /// <summary>
    /// Gets or sets the maximum angle in degrees, default is 360.
    /// </summary>
    /// <value>
    /// The maximum angle.
    /// </value>
    public double MaxAngle { get; set; }

    /// <summary>
    /// Gets or sets the total, it is the maximum value a pie slice can represent, when this property is null, the <see cref="Total"/> property
    /// will be calculated automatically based on the series data. Default value is null.
    /// </summary>
    public double? Total { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pie slices will be drawn clockwise, default is true.
    /// </summary>
    public bool IsClockwise { get; set; }
}
