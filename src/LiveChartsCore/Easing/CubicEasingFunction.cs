// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the CubicEasingFunction
    /// </summary>
    public static class CubicEasingFunction
    {
        /// <summary>
        /// The ease in.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float In(float t)
        {
            return t * t * t;
        }

        /// <summary>
        /// The ease out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float Out(float t)
        {
            return --t * t * t + 1;
        }

        /// <summary>
        /// The ease in out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? t * t * t : (t -= 2) * t * t + 2) / 2;
        }
    }
}
