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

namespace LiveChartsCore.Easing;

/// <summary>
/// Defines the CubicBezierEasingFunction.
/// </summary>
public static class CubicBezierEasingFunction
{
    private static readonly float s_iterations = 4f;
    private static readonly float s_minSlope = 0.001f;
    private static readonly float s_presicion = 0.0000001f;
    private static readonly float s_maxIterations = 10f;

    private static readonly int s_kSplineTableSize = 11;
    private static readonly float s_kSampleStepSize = 1.0f / (s_kSplineTableSize - 1.0f);

    /// <summary>
    /// Builds a bezier easing function.
    /// </summary>
    /// <param name="mX1">The m x1.</param>
    /// <param name="mY1">The m y1.</param>
    /// <param name="mX2">The m x2.</param>
    /// <param name="mY2">The m y2.</param>
    /// <returns></returns>
    /// <exception cref="Exception">Bezier x values must be in [0, 1] range</exception>
    public static Func<float, float> BuildBezierEasingFunction(float mX1, float mY1, float mX2, float mY2)
    {
        if (mX1 < 0 || mX1 > 1 || mX2 < 0 || mX2 > 1)
        {
            throw new Exception("Bezier x values must be in [0, 1] range");
        }

        if (mX1 == mY1 && mX2 == mY2)
        {
            return LinearEasing;
        }

        // Pre compute samples table
        var sampleValues = new float[s_kSplineTableSize];
        for (var i = 0; i < s_kSplineTableSize; ++i)
        {
            sampleValues[i] = CalcBezier(i * s_kSampleStepSize, mX1, mX2);
        }

        float getTForX(float aX)
        {
            var intervalStart = 0.0f;
            var currentSample = 1;
            var lastSample = s_kSplineTableSize - 1;

            for (; currentSample != lastSample && sampleValues[currentSample] <= aX; ++currentSample)
            {
                intervalStart += s_kSampleStepSize;
            }
            --currentSample;

            // Interpolate to provide an initial guess for t
            var dist = (aX - sampleValues[currentSample]) / (sampleValues[currentSample + 1] - sampleValues[currentSample]);
            var guessForT = intervalStart + dist * s_kSampleStepSize;

            var initialSlope = GetSlope(guessForT, mX1, mX2);
            return initialSlope >= s_minSlope
                ? NewtonRaphsonIterate(aX, guessForT, mX1, mX2)
                : initialSlope == 0.0f ? guessForT : BinarySubdivide(aX, intervalStart, intervalStart + s_kSampleStepSize, mX1, mX2);
        }

        return (t) =>
        {
            // Because JavaScript number are imprecise, we should guarantee the extremes are right.
            //if (t == 0f || t == 1f)
            //{
            //    return t;
            //}
            return CalcBezier(getTForX(t), mY1, mY2);
        };
    }

    private static float A(float aA1, float aA2) { return 1.0f - 3.0f * aA2 + 3.0f * aA1; }
    private static float B(float aA1, float aA2) { return 3.0f * aA2 - 6.0f * aA1; }
    private static float C(float aA1) { return 3.0f * aA1; }

    private static float CalcBezier(float aT, float aA1, float aA2) { return ((A(aA1, aA2) * aT + B(aA1, aA2)) * aT + C(aA1)) * aT; }

    private static float GetSlope(float aT, float aA1, float aA2) { return 3.0f * A(aA1, aA2) * aT * aT + 2.0f * B(aA1, aA2) * aT + C(aA1); }

    private static float BinarySubdivide(float aX, float aA, float aB, float mX1, float mX2)
    {
        float currentX;
        float currentT;
        var i = 0;
        do
        {
            currentT = aA + (aB - aA) / 2.0f;
            currentX = CalcBezier(currentT, mX1, mX2) - aX;
            if (currentX > 0.0)
            {
                aB = currentT;
            }
            else
            {
                aA = currentT;
            }
        } while (Math.Abs(currentX) > s_presicion && ++i < s_maxIterations);
        return currentT;
    }

    private static float NewtonRaphsonIterate(float aX, float aGuessT, float mX1, float mX2)
    {
        for (var i = 0; i < s_iterations; ++i)
        {
            var currentSlope = GetSlope(aGuessT, mX1, mX2);
            if (currentSlope == 0.0f)
            {
                return aGuessT;
            }
            var currentX = CalcBezier(aGuessT, mX1, mX2) - aX;
            aGuessT -= currentX / currentSlope;
        }
        return aGuessT;
    }

    private static float LinearEasing(float x)
    {
        return x;
    }
}
