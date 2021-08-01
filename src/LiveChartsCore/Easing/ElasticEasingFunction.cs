// This function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/elastic.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Defines the ElasticEasingFunction.
    /// </summary>
    public static class ElasticEasingFunction
    {
        private static readonly float tau = (float)(2 * Math.PI);

        /// <summary>
        /// The ease in.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="a">a.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static float In(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(a * Tpmt(-(--t)) * Math.Sin((s - t) / p));
            }
        }

        /// <summary>
        /// The ease out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="a">a.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static float Out(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(1 - a * Tpmt(t = +t) * Math.Sin((t + s) / p));
            }
        }

        /// <summary>
        /// The ease in out.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="a">a.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static float InOut(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (t = t * 2 - 1) < 0
                    ? (float)(a * Tpmt(-t) * Math.Sin((s - t) / p))
                    : (float)(2 - a * Tpmt(t) * Math.Sin((s + t) / p)) / 2f;
            }
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
