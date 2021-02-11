// this function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/back.js

namespace LiveChartsCore.Easing
{
    public static class BackEasingFunction
    {
        public static float In(float t, float s = 1.70158f)
        {
            return t * t * (s * (t - 1) + t);
        }

        public static float Out(float t, float s = 1.70158f)
        {
            return --t * t * ((t + 1) * s + t) + 1;
        }

        public static float InOut(float t, float s = 1.70158f)
        {
            return ((t *= 2) < 1 ? t * t * ((s + 1) * t - s) : (t -= 2) * t * ((s + 1) * t + s) + 2) / 2;
        }
    }
}
