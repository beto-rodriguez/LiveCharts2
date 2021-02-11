// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/exp.js

using System;

namespace LiveChartsCore.Easing
{
    public static class ExponentialEasingFunction
    {
        public static float In(float t)
        {
            return Tpmt(1 - +t);
        }

        public static float Out(float t)
        {
            return 1 - Tpmt(t);
        }

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
