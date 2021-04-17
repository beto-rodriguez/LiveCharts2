// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/bounce.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the BounceEasingFunction
    /// </summary>
    public static class BounceEasingFunction
    {
        private static float
             b1 = 4f / 11f,
             b2 = 6f / 11f,
             b3 = 8f / 11f,
             b4 = 3f / 4f,
             b5 = 9f / 11f,
             b6 = 10f / 11f,
             b7 = 15f / 16f,
             b8 = 21f / 22f,
             b9 = 63f / 64f,
             b0 = 1f / b1 / b1;

        /// <summary>
        /// the in easing.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float In(float t)
        {
            return 1 - Out(1 - t);
        }

        /// <summary>
        /// the out easing.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float Out(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        /// <summary>
        /// the in out easing.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? 1 - Out(1 - t) : Out(t - 1) + 1) / 2f;
        }
    }
}
