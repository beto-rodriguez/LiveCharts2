// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/bounce.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the BounceEasingFunction
    /// </summary>
    public static class BounceEasingFunction
    {
        private static readonly float s_b1 = 4f / 11f;
        private static readonly float s_b2 = 6f / 11f;
        private static readonly float s_b3 = 8f / 11f;
        private static readonly float s_b4 = 3f / 4f;
        private static readonly float s_b5 = 9f / 11f;
        private static readonly float s_b6 = 10f / 11f;
        private static readonly float s_b7 = 15f / 16f;
        private static readonly float s_b8 = 21f / 22f;
        private static readonly float s_b9 = 63f / 64f;
        private static readonly float s_b0 = 1f / s_b1 / s_b1;

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
            return (t = +t) < s_b1
                ? s_b0 * t * t : t < s_b3 ? s_b0 * (t -= s_b2) * t + s_b4 : t < s_b6 ? s_b0 * (t -= s_b5) * t + s_b7
                : s_b0 * (t -= s_b8) * t + s_b9;
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
