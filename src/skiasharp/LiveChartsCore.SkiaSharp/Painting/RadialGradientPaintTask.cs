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

/// <inheritdoc cref="RadialGradientPaintTask"/>
[Obsolete("Renamed to RadialGradientPaint")]
public class RadialGradientPaintTask : RadialGradientPaint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGradientPaint"/> class.
    /// </summary>
    /// <param name="gradientStops">The gradient stops.</param>
    /// <param name="center">
    /// The center point of the gradient, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end,
    /// default is (0.5, 0.5).
    /// </param>
    /// <param name="radius">
    /// The radius, in the range of 0 to 1, where 1 is the minimum of both Width and Height of the chart, default is 0.5.
    /// </param>
    /// <param name="colorPos">
    /// An array of integers in the range of 0 to 1.
    /// These integers indicate the relative positions of the colors, You can set that argument to null to equally
    /// space the colors, default is null.
    /// </param>
    /// <param name="tileMode">
    /// The shader tile mode, default is <see cref="SKShaderTileMode.Repeat"/>.
    /// </param>
    public RadialGradientPaintTask(
        SKColor[] gradientStops,
        SKPoint? center = null,
        float radius = 0.5f,
        float[]? colorPos = null,
        SKShaderTileMode tileMode = SKShaderTileMode.Repeat)
        : base(gradientStops, center, radius, colorPos, tileMode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGradientPaint"/> class.
    /// </summary>
    /// <param name="centerColor">Color of the center.</param>
    /// <param name="outerColor">Color of the outer.</param>
    public RadialGradientPaintTask(SKColor centerColor, SKColor outerColor)
        : this(new[] { centerColor, outerColor }) { }
}
