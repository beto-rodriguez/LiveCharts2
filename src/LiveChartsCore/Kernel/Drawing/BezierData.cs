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

namespace LiveChartsCore.Kernel.Drawing;

/// <summary>
/// Defines the bezier data class.
/// </summary>
public class BezierData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BezierData"/> class.
    /// </summary>
    /// <param name="chartPoint">The chart point.</param>
    public BezierData(ChartPoint chartPoint)
    {
        TargetPoint = chartPoint;
        X0 = Y0 = X1 = Y1 = X2 = Y2 = 0;
    }

    /// <summary>
    /// Gets or sets the target point.
    /// </summary>
    /// <value>
    /// The target point.
    /// </value>
    public ChartPoint TargetPoint { get; set; }

    /// <summary>
    /// Gets or sets the x0.
    /// </summary>
    /// <value>
    /// The x0.
    /// </value>
    public double X0 { get; set; }

    /// <summary>
    /// Gets or sets the y0.
    /// </summary>
    /// <value>
    /// The y0.
    /// </value>
    public double Y0 { get; set; }

    /// <summary>
    /// Gets or sets the x1.
    /// </summary>
    /// <value>
    /// The x1.
    /// </value>
    public double X1 { get; set; }

    /// <summary>
    /// Gets or sets the y1.
    /// </summary>
    /// <value>
    /// The y1.
    /// </value>
    public double Y1 { get; set; }

    /// <summary>
    /// Gets or sets the x2.
    /// </summary>
    /// <value>
    /// The x2.
    /// </value>
    public double X2 { get; set; }

    /// <summary>
    /// Gets or sets the y2.
    /// </summary>
    /// <value>
    /// The y2.
    /// </value>
    public double Y2 { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the next point is empty.
    /// </summary>
    public bool IsNextEmpty { get; set; }
}

