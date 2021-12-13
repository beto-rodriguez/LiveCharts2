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

using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting.Effects;

/// <summary>
/// Creates a stroke dash effect.
/// </summary>
/// <seealso cref="PathEffect" />
public class DashEffect : PathEffect
{
    private readonly float[] _dashArray;
    private readonly float _phase = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashEffect"/> class.
    /// </summary>
    public DashEffect(float[] dashArray, float phase = 0)
    {
        _dashArray = dashArray;
        _phase = phase;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override PathEffect Clone()
    {
        return new DashEffect(_dashArray, _phase);
    }

    /// <summary>
    /// Creates the path effect.
    /// </summary>
    /// <param name="drawingContext">The drawing context.</param>
    public override void CreateEffect(SkiaSharpDrawingContext drawingContext)
    {
        SKPathEffect = SKPathEffect.CreateDash(_dashArray, _phase);
    }
}
