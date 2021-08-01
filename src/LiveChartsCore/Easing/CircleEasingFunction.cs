// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the CircleEasingFunction
    /// </summary>
    public static class CircleEasingFunction
    {
        /// <summary>
        /// the ease in.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float In(float t)
        {
            unchecked
            {
                return (float)(1 - Math.Sqrt(1 - (t * t)));
            }
        }

        /// <summary>
        /// the ease out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float Out(float t)
        {
            unchecked
            {
                return (float)Math.Sqrt(1 - (--t * t));
            }
        }

        /// <summary>
        /// the ease in out
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float InOut(float t)
        {
            return (float)((t *= 2) <= 1 ? 1 - Math.Sqrt(1 - (t * t)) : Math.Sqrt(1 - ((t -= 2) * t)) + 1) / 2f;
        }
    }
}
