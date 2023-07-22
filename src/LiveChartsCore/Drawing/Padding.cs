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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines the padding measure helped class,.
/// </summary>
public class Padding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> class.
    /// </summary>
    public Padding()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> class.
    /// </summary>
    /// <param name="padding">The padding.</param>
    public Padding(double padding)
    {
        var p = (float)padding;
        Left = p;
        Top = p;
        Right = p;
        Bottom = p;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> class.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    public Padding(double x, double y)
    {
        var px = (float)x;
        var py = (float)y;
        Left = px;
        Top = py;
        Right = px;
        Bottom = py;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> class.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="top">The top.</param>
    /// <param name="right">The right.</param>
    /// <param name="bottom">The bottom.</param>
    public Padding(double left, double top, double right, double bottom)
    {
        Left = (float)left;
        Top = (float)top;
        Right = (float)right;
        Bottom = (float)bottom;
    }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>
    /// The left.
    /// </value>
    public float Left { get; set; }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>
    /// The right.
    /// </value>
    public float Right { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    public float Top { get; set; }

    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>
    /// The bottom.
    /// </value>
    public float Bottom { get; set; }
}
