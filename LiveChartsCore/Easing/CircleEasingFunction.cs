// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

using System;

namespace LiveChartsCore.Easing
{
    public static class CircleEasingFunction
    {
        public static float In(float t)
        {
            unchecked
            {
                return (float)(1 - Math.Sqrt(1 - t * t));
            }
        }

        public static float Out(float t)
        {
            unchecked
            {
                return (float)Math.Sqrt(1 - --t * t);
            }
        }

        public static float InOut(float t)
        {
            return (float)((t *= 2) <= 1 ? 1 - Math.Sqrt(1 - t * t) : Math.Sqrt(1 - (t -= 2) * t) + 1) / 2f;
        }
    }
}
