// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/poly.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the PolinominalEasingFunction.
    /// </summary>
    public static class PolinominalEasingFunction
    {
        /// <summary>
        /// The ease in.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public static float In(float t, float e = 3f)
        {
            unchecked
            {
                return (float)Math.Pow(t, e);
            }
        }

        /// <summary>
        /// The ease out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public static float Out(float t, float e = 3f)
        {
            unchecked
            {
                return (float)(1 - Math.Pow(1 - t, e));
            }
        }

        /// <summary>
        /// The ease in out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public static float InOut(float t, float e = 3f)
        {
            unchecked
            {
                return (float)((t *= 2) <= 1 ? Math.Pow(t, e) : 2 - Math.Pow(2 - t, e)) / 2f;
            }
        }
    }
}
