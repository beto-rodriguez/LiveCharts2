using LiveChartsCore.Easing;
using System;

namespace LiveChartsCore
{
    public static class EasingFunctions
    {
        public static Func<float, float> BackIn => t => BackEasingFunction.In(t);
        public static Func<float, float> BackOut => t => BackEasingFunction.Out(t);
        public static Func<float, float> BackInOut => t => BackEasingFunction.InOut(t);

        public static Func<float, float> BounceIn => BounceEasingFunction.In;
        public static Func<float, float> BounceOut => BounceEasingFunction.Out;
        public static Func<float, float> BounceInOut => BounceEasingFunction.InOut;

        public static Func<float, float> CircleIn => CircleEasingFunction.In;
        public static Func<float, float> CircleOut => CircleEasingFunction.Out;
        public static Func<float, float> CircleInOut => CircleEasingFunction.InOut;

        public static Func<float, float> CubicIn => CubicEasingFunction.In;
        public static Func<float, float> CubicOut => CubicEasingFunction.Out;
        public static Func<float, float> CubicInOut => CubicEasingFunction.InOut;

        public static Func<float, float> Ease => BuildCubicBezier(0.25f, 0.1f, 0.25f, 1f);
        public static Func<float, float> EaseIn => BuildCubicBezier(0.42f, 0f, 1f, 1f);
        public static Func<float, float> EaseOut => BuildCubicBezier(0f, 0f, 0.58f, 1f);
        public static Func<float, float> EaseInOut => BuildCubicBezier(0.42f, 0f, 0.58f, 1f);

        public static Func<float, float> ElasticIn => t => ElasticEasingFunction.In(t);
        public static Func<float, float> ElasticOut => t => ElasticEasingFunction.Out(t);
        public static Func<float, float> ElasticInOut => t => ElasticEasingFunction.InOut(t);

        public static Func<float, float> ExponentialIn => ExponentialEasingFunction.In;
        public static Func<float, float> ExponentialOut => ExponentialEasingFunction.Out;
        public static Func<float, float> ExponentialInOut => ExponentialEasingFunction.InOut;

        public static Func<float, float> Lineal => t => t;

        public static Func<float, float> PolinominalIn => t => PolinominalEasingFunction.In(t);
        public static Func<float, float> PolinominalOut => t => PolinominalEasingFunction.Out(t);
        public static Func<float, float> PolinominalInOut => t => PolinominalEasingFunction.InOut(t);

        public static Func<float, float> QuadraticIn => t => t * t;
        public static Func<float, float> QuadraticOut => t => t * (2 - t);
        public static Func<float, float> QuadraticInOut => t => ((t *= 2) <= 1 ? t * t : --t * (2 - t) + 1) / 2f;

        public static Func<float, float> SinIn => t => +t == 1 ? 1 : unchecked((float)(1 - Math.Cos(t * Math.PI / 2d)));
        public static Func<float, float> SinOut => t => unchecked((float)Math.Sin(t * Math.PI / 2d));
        public static Func<float, float> SinInOut => t => unchecked((float)(1 - Math.Cos(Math.PI * t))) / 2f;

        public static Func<float, Func<float, float>> BuildCustomBackIn =>
            overshoot => t => BackEasingFunction.In(t, overshoot);
        public static Func<float, Func<float, float>> BuildCustomBackOut =>
            overshoot => t => BackEasingFunction.Out(t, overshoot);
        public static Func<float, Func<float, float>> BuildCustomBackInOut =>
            overshoot => t => BackEasingFunction.InOut(t, overshoot);

        public static Func<float, float, Func<float, float>> BuildCustomElasticIn =>
            (amplitude, period) => t => ElasticEasingFunction.In(t, amplitude, period);
        public static Func<float, float, Func<float, float>> BuildCustomElasticOut =>
            (amplitude, period) => t => ElasticEasingFunction.Out(t, amplitude, period);
        public static Func<float, float, Func<float, float>> BuildCustomElasticInOut =>
            (amplitude, period) => t => ElasticEasingFunction.InOut(t, amplitude, period);

        public static Func<float, Func<float, float>> BuildCustomPolinominalIn =>
            exponent => t => PolinominalEasingFunction.In(t, exponent);
        public static Func<float, Func<float, float>> BuildCustomPolinominalOut =>
            exponent => t => PolinominalEasingFunction.Out(t, exponent);
        public static Func<float, Func<float, float>> BuildCustomPolinominalInOut =>
            exponent => t => PolinominalEasingFunction.InOut(t, exponent);

        public static Func<float, float, float, float, Func<float, float>> BuildCubicBezier =>
             (mX1, mY1, mX2, mY2) => CubicBezierEasingFunction.BuildBezierEasingFunction(mX1, mY1, mX2, mY2);
    }
}
