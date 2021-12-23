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

namespace LiveChartsCore.Measure;

/// <summary>
///Defines the margin class.
/// </summary>
public class Margin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Margin"/> class.
    /// </summary>
    public Margin()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Margin"/> class.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="top">The top.</param>
    /// <param name="right">The right.</param>
    /// <param name="bottom">The bottom.</param>
    public Margin(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Margin"/> class.
    /// </summary>
    /// <param name="all">The margin for top, left, right and bottom.</param>
    public Margin(float all) : this(all, all, all, all) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Margin"/> class.
    /// </summary>
    /// <param name="x">The margin for left and right.</param>
    /// <param name="y">The margin for top and bottom.</param>
    public Margin(float x, float y) : this(x, y, x, y) { }

    /// <summary>
    /// Indicates that the margin should be calculated by the library.
    /// </summary>
    public static float Auto => float.NaN;

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>
    /// The left.
    /// </value>
    public float Left { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    public float Top { get; set; }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>
    /// The right.
    /// </value>
    public float Right { get; set; }

    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>
    /// The bottom.
    /// </value>
    public float Bottom { get; set; }

    /// <summary>
    /// Evaluates whether the given margin is automatic.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsAuto(float value)
    {
        return float.IsNaN(value);
    }
}
