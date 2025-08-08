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

namespace LiveChartsCore.SkiaSharpView.Painting.Effects;

/// <summary>
/// Creates a stroke dash effect.
/// </summary>
/// <seealso cref="PathEffect" />
/// <remarks>
/// Initializes a new instance of the <see cref="DashEffect"/> class.
/// </remarks>
public class DashEffect(float[] dashArray, float phase = 0) : PathEffect
{
    private float[] DashArray { get; set; } = dashArray;
    private float Phase { get; set; } = phase;

    /// <inheritdoc cref="PathEffect.Clone"/>
    public override PathEffect Clone() => new DashEffect(DashArray, Phase);

    /// <inheritdoc cref="PathEffect.CreateEffect()"/>
    public override void CreateEffect() =>
        SKPathEffect = SKPathEffect.CreateDash(DashArray, Phase);

    /// <inheritdoc cref="PathEffect.Transitionate(float, PathEffect)"/>
    public override PathEffect? Transitionate(float progress, PathEffect? target)
    {
        if (target is not DashEffect dashEffect) return target;

        var clone = (DashEffect)Clone();

        if (DashArray.Length != dashEffect.DashArray.Length)
            throw new Exception("The dash arrays must have the same length");

        for (var i = 0; i < DashArray.Length; i++)
            clone.DashArray[i] = DashArray[i] + (dashEffect.DashArray[i] - DashArray[i]) * progress;

        clone.Phase = Phase + (dashEffect.Phase - Phase) * progress;

        return clone;
    }
}
