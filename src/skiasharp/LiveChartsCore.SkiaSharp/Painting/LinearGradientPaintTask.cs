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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <inheritdoc cref="LinearGradientPaint"/>
[Obsolete("Renamed to LinearGradientPaint")]
public class LinearGradientPaintTask : LinearGradientPaint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientPaint"/> class.
    /// </summary>
    /// <param name="gradientStops">The gradient stops.</param>
    /// <param name="startPoint">
    /// The start point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
    /// </param>
    /// <param name="endPoint">
    /// The end point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
    /// </param>
    /// <param name="colorPos">
    /// An array of integers in the range of 0 to 1.
    /// These integers indicate the relative positions of the colors, You can set that argument to null to equally
    /// space the colors, default is null.
    /// </param>
    /// <param name="tileMode">
    /// The shader tile mode, default is <see cref="SKShaderTileMode.Repeat"/>.
    /// </param>
    public LinearGradientPaintTask(
        SKColor[] gradientStops,
        SKPoint startPoint,
        SKPoint endPoint,
        float[]? colorPos = null,
        SKShaderTileMode tileMode = SKShaderTileMode.Repeat)
            : base(gradientStops, startPoint, endPoint, colorPos, tileMode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientPaint"/> class.
    /// </summary>
    /// <param name="gradientStops">The gradient stops.</param>
    public LinearGradientPaintTask(SKColor[] gradientStops)
        : this(gradientStops, s_defaultStartPoint, s_defaultEndPoint) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientPaint"/> class.
    /// </summary>
    /// <param name="startColor">The start color.</param>
    /// <param name="endColor">The end color.</param>
    /// <param name="startPoint">
    /// The start point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
    /// </param>
    /// <param name="endPoint">
    /// The end point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
    /// </param>
    public LinearGradientPaintTask(SKColor startColor, SKColor endColor, SKPoint startPoint, SKPoint endPoint)
            : this(new[] { startColor, endColor }, startPoint, endPoint) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientPaint"/> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public LinearGradientPaintTask(SKColor start, SKColor end)
        : this(start, end, s_defaultStartPoint, s_defaultEndPoint) { }
}
