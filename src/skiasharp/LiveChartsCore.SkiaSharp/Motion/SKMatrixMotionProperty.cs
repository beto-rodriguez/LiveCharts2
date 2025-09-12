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

using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Motion;

/// <summary>
/// Defines a motion property to handle the <see cref="SKMatrix"/> type.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ColorMotionProperty"/> class.
/// </remarks>
/// <param name="defaultValue">The default value.</param>
public class SKMatrixMotionProperty(SKMatrix defaultValue)
    : MotionProperty<SKMatrix>(defaultValue)
{
    /// <inheritdoc cref="MotionProperty{T}.CanTransitionate"/>
    protected override bool CanTransitionate => true;

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)"/>
    protected override SKMatrix OnGetMovement(float progress)
    {
        return new SKMatrix(
            FromValue.ScaleX + progress * (ToValue.ScaleX - FromValue.ScaleX),
            FromValue.SkewX + progress * (ToValue.SkewX - FromValue.SkewX),
            FromValue.TransX + progress * (ToValue.TransX - FromValue.TransX),
            FromValue.SkewY + progress * (ToValue.SkewY - FromValue.SkewY),
            FromValue.ScaleY + progress * (ToValue.ScaleY - FromValue.ScaleY),
            FromValue.TransY + progress * (ToValue.TransY - FromValue.TransY),
            FromValue.Persp0 + progress * (ToValue.Persp0 - FromValue.Persp0),
            FromValue.Persp1 + progress * (ToValue.Persp1 - FromValue.Persp1),
            FromValue.Persp2 + progress * (ToValue.Persp2 - FromValue.Persp2));
    }
}
