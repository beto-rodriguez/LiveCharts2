// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Easing;

namespace LiveChartsCore;

/// <summary>
/// A set of predefined easing functions.
/// </summary>
public static class EasingFunctions
{
    /// <summary>
    /// Gets the back in.
    /// </summary>
    /// <value>
    /// The back in.
    /// </value>
    public static Func<float, float> BackIn => t => BackEasingFunction.In(t);

    /// <summary>
    /// Gets the back out.
    /// </summary>
    /// <value>
    /// The back out.
    /// </value>
    public static Func<float, float> BackOut => t => BackEasingFunction.Out(t);

    /// <summary>
    /// Gets the back in out.
    /// </summary>
    /// <value>
    /// The back in out.
    /// </value>
    public static Func<float, float> BackInOut => t => BackEasingFunction.InOut(t);

    /// <summary>
    /// Gets the bounce in.
    /// </summary>
    /// <value>
    /// The bounce in.
    /// </value>
    public static Func<float, float> BounceIn => BounceEasingFunction.In;

    /// <summary>
    /// Gets the bounce out.
    /// </summary>
    /// <value>
    /// The bounce out.
    /// </value>
    public static Func<float, float> BounceOut => BounceEasingFunction.Out;

    /// <summary>
    /// Gets the bounce in out.
    /// </summary>
    /// <value>
    /// The bounce in out.
    /// </value>
    public static Func<float, float> BounceInOut => BounceEasingFunction.InOut;

    /// <summary>
    /// Gets the circle in.
    /// </summary>
    /// <value>
    /// The circle in.
    /// </value>
    public static Func<float, float> CircleIn => CircleEasingFunction.In;

    /// <summary>
    /// Gets the circle out.
    /// </summary>
    /// <value>
    /// The circle out.
    /// </value>
    public static Func<float, float> CircleOut => CircleEasingFunction.Out;

    /// <summary>
    /// Gets the circle in out.
    /// </summary>
    /// <value>
    /// The circle in out.
    /// </value>
    public static Func<float, float> CircleInOut => CircleEasingFunction.InOut;

    /// <summary>
    /// Gets the cubic in.
    /// </summary>
    /// <value>
    /// The cubic in.
    /// </value>
    public static Func<float, float> CubicIn => CubicEasingFunction.In;

    /// <summary>
    /// Gets the cubic out.
    /// </summary>
    /// <value>
    /// The cubic out.
    /// </value>
    public static Func<float, float> CubicOut => CubicEasingFunction.Out;

    /// <summary>
    /// Gets the cubic in out.
    /// </summary>
    /// <value>
    /// The cubic in out.
    /// </value>
    public static Func<float, float> CubicInOut => CubicEasingFunction.InOut;

    /// <summary>
    /// Gets the ease.
    /// </summary>
    /// <value>
    /// The ease.
    /// </value>
    public static Func<float, float> Ease => BuildCubicBezier(0.25f, 0.1f, 0.25f, 1f);

    /// <summary>
    /// Gets the ease in.
    /// </summary>
    /// <value>
    /// The ease in.
    /// </value>
    public static Func<float, float> EaseIn => BuildCubicBezier(0.42f, 0f, 1f, 1f);

    /// <summary>
    /// Gets the ease out.
    /// </summary>
    /// <value>
    /// The ease out.
    /// </value>
    public static Func<float, float> EaseOut => BuildCubicBezier(0f, 0f, 0.58f, 1f);

    /// <summary>
    /// Gets the ease in out.
    /// </summary>
    /// <value>
    /// The ease in out.
    /// </value>
    public static Func<float, float> EaseInOut => BuildCubicBezier(0.42f, 0f, 0.58f, 1f);

    /// <summary>
    /// Gets the elastic in.
    /// </summary>
    /// <value>
    /// The elastic in.
    /// </value>
    public static Func<float, float> ElasticIn => t => ElasticEasingFunction.In(t);

    /// <summary>
    /// Gets the elastic out.
    /// </summary>
    /// <value>
    /// The elastic out.
    /// </value>
    public static Func<float, float> ElasticOut => t => ElasticEasingFunction.Out(t);

    /// <summary>
    /// Gets the elastic in out.
    /// </summary>
    /// <value>
    /// The elastic in out.
    /// </value>
    public static Func<float, float> ElasticInOut => t => ElasticEasingFunction.InOut(t);

    /// <summary>
    /// Gets the exponential in.
    /// </summary>
    /// <value>
    /// The exponential in.
    /// </value>
    public static Func<float, float> ExponentialIn => ExponentialEasingFunction.In;

    /// <summary>
    /// Gets the exponential out.
    /// </summary>
    /// <value>
    /// The exponential out.
    /// </value>
    public static Func<float, float> ExponentialOut => ExponentialEasingFunction.Out;

    /// <summary>
    /// Gets the exponential in out.
    /// </summary>
    /// <value>
    /// The exponential in out.
    /// </value>
    public static Func<float, float> ExponentialInOut => ExponentialEasingFunction.InOut;

    /// <summary>
    /// Gets the lineal.
    /// </summary>
    /// <value>
    /// The lineal.
    /// </value>
    public static Func<float, float> Lineal => t => t;

    /// <summary>
    /// Gets the polinominal in.
    /// </summary>
    /// <value>
    /// The polinominal in.
    /// </value>
    public static Func<float, float> PolinominalIn => t => PolinominalEasingFunction.In(t);

    /// <summary>
    /// Gets the polinominal out.
    /// </summary>
    /// <value>
    /// The polinominal out.
    /// </value>
    public static Func<float, float> PolinominalOut => t => PolinominalEasingFunction.Out(t);

    /// <summary>
    /// Gets the polinominal in out.
    /// </summary>
    /// <value>
    /// The polinominal in out.
    /// </value>
    public static Func<float, float> PolinominalInOut => t => PolinominalEasingFunction.InOut(t);

    /// <summary>
    /// Gets the quadratic in.
    /// </summary>
    /// <value>
    /// The quadratic in.
    /// </value>
    public static Func<float, float> QuadraticIn => t => t * t;

    /// <summary>
    /// Gets the quadratic out.
    /// </summary>
    /// <value>
    /// The quadratic out.
    /// </value>
    public static Func<float, float> QuadraticOut => t => t * (2 - t);

    /// <summary>
    /// Gets the quadratic in out.
    /// </summary>
    /// <value>
    /// The quadratic in out.
    /// </value>
    public static Func<float, float> QuadraticInOut => t => ((t *= 2) <= 1 ? t * t : --t * (2 - t) + 1) / 2f;

    /// <summary>
    /// Gets the sin in.
    /// </summary>
    /// <value>
    /// The sin in.
    /// </value>
    public static Func<float, float> SinIn => t => +t == 1 ? 1 : unchecked((float)(1 - Math.Cos(t * Math.PI / 2d)));

    /// <summary>
    /// Gets the sin out.
    /// </summary>
    /// <value>
    /// The sin out.
    /// </value>
    public static Func<float, float> SinOut => t => unchecked((float)Math.Sin(t * Math.PI / 2d));

    /// <summary>
    /// Gets the sin in out.
    /// </summary>
    /// <value>
    /// The sin in out.
    /// </value>
    public static Func<float, float> SinInOut => t => unchecked((float)(1 - Math.Cos(Math.PI * t))) / 2f;

    /// <summary>
    /// Gets a fuction based on the given <see cref="KeyFrame"/> collection.
    /// </summary>
    public static Func<KeyFrame[], Func<float, float>> BuildFunctionUsingKeyFrames =>
        keyFrames =>
        {
            if (keyFrames.Length < 2) throw new Exception("At least 2 key frames are required.");
            if (keyFrames[keyFrames.Length - 1].Time < 1)
            {
                var newKeyFrames = new List<KeyFrame>(keyFrames)
                {
                        new()
                        {
                            Time = 1,
                            Value = keyFrames[keyFrames.Length - 1].Value
                        }
                };
                keyFrames = newKeyFrames.ToArray();
            }

            return t =>
            {
                var i = 0;
                var current = keyFrames[i];
                var next = keyFrames[i + 1];

                while (next.Time < t && i < keyFrames.Length - 2)
                {
                    i++;
                    current = keyFrames[i];
                    next = keyFrames[i + 1];
                }

                var dt = next.Time - current.Time;
                var dv = next.Value - current.Value;

                var p = (t - current.Time) / dt;

                return current.Value + next.EasingFunction(p) * dv;
            };
        };

    /// <summary>
    /// Gets the build custom back in.
    /// </summary>
    /// <value>
    /// The build custom back in.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomBackIn =>
        overshoot => t => BackEasingFunction.In(t, overshoot);

    /// <summary>
    /// Gets the build custom back out.
    /// </summary>
    /// <value>
    /// The build custom back out.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomBackOut =>
        overshoot => t => BackEasingFunction.Out(t, overshoot);

    /// <summary>
    /// Gets the build custom back in out.
    /// </summary>
    /// <value>
    /// The build custom back in out.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomBackInOut =>
        overshoot => t => BackEasingFunction.InOut(t, overshoot);

    /// <summary>
    /// Gets the build custom elastic in.
    /// </summary>
    /// <value>
    /// The build custom elastic in.
    /// </value>
    public static Func<float, float, Func<float, float>> BuildCustomElasticIn =>
        (amplitude, period) => t => ElasticEasingFunction.In(t, amplitude, period);

    /// <summary>
    /// Gets the build custom elastic out.
    /// </summary>
    /// <value>
    /// The build custom elastic out.
    /// </value>
    public static Func<float, float, Func<float, float>> BuildCustomElasticOut =>
        (amplitude, period) => t => ElasticEasingFunction.Out(t, amplitude, period);

    /// <summary>
    /// Gets the build custom elastic in out.
    /// </summary>
    /// <value>
    /// The build custom elastic in out.
    /// </value>
    public static Func<float, float, Func<float, float>> BuildCustomElasticInOut =>
        (amplitude, period) => t => ElasticEasingFunction.InOut(t, amplitude, period);

    /// <summary>
    /// Gets the build custom polinominal in.
    /// </summary>
    /// <value>
    /// The build custom polinominal in.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomPolinominalIn =>
        exponent => t => PolinominalEasingFunction.In(t, exponent);

    /// <summary>
    /// Gets the build custom polinominal out.
    /// </summary>
    /// <value>
    /// The build custom polinominal out.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomPolinominalOut =>
        exponent => t => PolinominalEasingFunction.Out(t, exponent);

    /// <summary>
    /// Gets the build custom polinominal in out.
    /// </summary>
    /// <value>
    /// The build custom polinominal in out.
    /// </value>
    public static Func<float, Func<float, float>> BuildCustomPolinominalInOut =>
        exponent => t => PolinominalEasingFunction.InOut(t, exponent);

    /// <summary>
    /// Gets the build cubic bezier.
    /// </summary>
    /// <value>
    /// The build cubic bezier.
    /// </value>
    public static Func<float, float, float, float, Func<float, float>> BuildCubicBezier =>
         (mX1, mY1, mX2, mY2) => CubicBezierEasingFunction.BuildBezierEasingFunction(mX1, mY1, mX2, mY2);
}
