// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

namespace LiveChartsCore.Easing
{
    public static class CubicEasingFunction
    {
        public static float In(float t)
        {
            return t * t * t;
        }

        public static float Out(float t)
        {
            return --t * t * t + 1;
        }

        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? t * t * t : (t -= 2) * t * t + 2) / 2;
        }
    }
}
