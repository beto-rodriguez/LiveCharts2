// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/poly.js

using System;

namespace LiveChartsCore.Easing
{
    public static class PolinominalEasingFunction
    {
        public static float In(float t, float e = 3f)
        {
            unchecked
            {
                return (float)Math.Pow(t, e);
            }
        }

        public static float Out(float t, float e = 3f)
        {
            unchecked
            {
                return (float)(1 - Math.Pow(1 - t, e));
            }
        }

        public static float InOut(float t, float e = 3f)
        {
            unchecked
            {
                return (float)((t *= 2) <= 1 ? Math.Pow(t, e) : 2 - Math.Pow(2 - t, e)) / 2f;
            }
        }
    }
}
