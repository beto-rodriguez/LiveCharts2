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
using LiveChartsCore.Kernel;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// A helper class to build delayed animations.
    /// </summary>
    public class DelayedFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedFunction"/> class.
        /// </summary>
        /// <param name="baseFunction">The base function.</param>
        /// <param name="point">The point.</param>
        /// <param name="perPointDelay">The per point delay.</param>
        public DelayedFunction(Func<float, float> baseFunction, ChartPoint point, float perPointDelay = 10)
        {
            var visual = point.Context.Visual;
            var chart = point.Context.Chart;

            var delay = point.Context.Index * perPointDelay;
            var speed = (float)chart.AnimationsSpeed.TotalMilliseconds + delay;

            var d = delay / speed;

            Function = p =>
            {
                if (p <= d) return 0;
                var p2 = (p - d) / (1 - d);
                return baseFunction(p2);
            };
            Speed = TimeSpan.FromMilliseconds(speed);
        }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public Func<float, float> Function { get; }

        /// <summary>
        /// Gets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public TimeSpan Speed { get; }
    }
}
