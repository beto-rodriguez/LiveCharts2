// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/exp.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the ExponentialEasingFunction.
    /// </summary>
    public static class ExponentialEasingFunction
    {
        /// <summary>
        /// The ease in.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float In(float t)
        {
            return Tpmt(1 - +t);
        }

        /// <summary>
        /// The ease out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float Out(float t)
        {
            return 1 - Tpmt(t);
        }

        /// <summary>
        /// The ease in out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? Tpmt(1 - t) : 2 - Tpmt(t - 1)) / 2;
        }

        private static float Tpmt(float x)
        {
            unchecked
            {
                return (float)((Math.Pow(2, -10 * x) - 0.0009765625) * 1.0009775171065494);
            }
        }
    }
}
