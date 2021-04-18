// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.SkiaSharpView.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Motion
{
    /// <summary>
    /// An wapper to enable dash path effects animations for skia sharp path effects.
    /// </summary>
    /// <seealso cref="LiveChartsCore.SkiaSharpView.Motion.Composed.PathEffect" />
    public class DashPathEffect : PathEffect
    {
        private readonly float[] dashArray;
        private readonly float phase;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashPathEffect"/> class.
        /// </summary>
        /// <param name="dashArray">The dash array.</param>
        /// <param name="phase">The phase.</param>
        public DashPathEffect(float[] dashArray, float phase)
        {
            this.dashArray = dashArray;
            this.phase = phase;
        }

        /// <inheritdoc cref="PathEffect" />
        public override SKPathEffect GetSKPath()
        {
            return SKPathEffect.CreateDash(dashArray, phase);
        }

        /// <inheritdoc cref="PathEffect" />
        public override PathEffect InterpolateFrom(PathEffect from, float progress)
        {
            var fromDashEffect = (DashPathEffect)from;
            return new DashPathEffect(dashArray, fromDashEffect.phase + progress * (phase - 0));
        }
    }
}
