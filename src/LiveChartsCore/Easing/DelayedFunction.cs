using LiveChartsCore.Kernel;
using System;

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
