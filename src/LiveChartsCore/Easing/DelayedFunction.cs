using LiveChartsCore.Context;
using System;

namespace LiveChartsCore.Easing
{
    public class DelayedFunction
    {
        public DelayedFunction(Func<float, float> baseFunction, IChartPoint point, float perPointDelay = 10)
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

        public Func<float, float> Function { get; }
        public TimeSpan Speed { get; }
    }
}
