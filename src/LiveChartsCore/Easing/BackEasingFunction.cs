// this function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/back.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the BackEasingFunction
    /// </summary>
    public static class BackEasingFunction
    {
        /// <summary>
        /// The in easing
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static float In(float t, float s = 1.70158f)
        {
            return t * t * (s * (t - 1) + t);
        }

        /// <summary>
        /// the out easing
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static float Out(float t, float s = 1.70158f)
        {
            return --t * t * ((t + 1) * s + t) + 1;
        }

        /// <summary>
        /// the in out easing
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static float InOut(float t, float s = 1.70158f)
        {
            return ((t *= 2) < 1 ? t * t * ((s + 1) * t - s) : (t -= 2) * t * ((s + 1) * t + s) + 2) / 2;
        }
    }
}
