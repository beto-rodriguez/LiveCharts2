// this function is inspired on
// https://github.com/gre/bezier-easing/blob/master/src/index.js

using System;

namespace LiveChartsCore.Easing
{
    public static class CubicBezierEasingFunction
    {
        private static readonly float NEWTON_ITERATIONS = 4f;
        private static readonly float NEWTON_MIN_SLOPE = 0.001f;
        private static readonly float SUBDIVISION_PRECISION = 0.0000001f;
        private static readonly float SUBDIVISION_MAX_ITERATIONS = 10f;

        private static readonly int kSplineTableSize = 11;
        private static readonly float kSampleStepSize = 1.0f / (kSplineTableSize - 1.0f);

        public static Func<float, float> BuildBezierEasingFunction(float mX1, float mY1, float mX2, float mY2)
        {
            if (!(0 <= mX1 && mX1 <= 1 && 0 <= mX2 && mX2 <= 1))
            {
                throw new Exception("Bezier x values must be in [0, 1] range");
            }

            if (mX1 == mY1 && mX2 == mY2)
            {
                return LinearEasing;
            }

            // Precompute samples table
            var sampleValues = new float[kSplineTableSize];
            for (var i = 0; i < kSplineTableSize; ++i)
            {
                sampleValues[i] = CalcBezier(i * kSampleStepSize, mX1, mX2);
            }

            float getTForX(float aX)
            {
                var intervalStart = 0.0f;
                var currentSample = 1;
                var lastSample = kSplineTableSize - 1;

                for (; currentSample != lastSample && sampleValues[currentSample] <= aX; ++currentSample)
                {
                    intervalStart += kSampleStepSize;
                }
                --currentSample;

                // Interpolate to provide an initial guess for t
                var dist = (aX - sampleValues[currentSample]) / (sampleValues[currentSample + 1] - sampleValues[currentSample]);
                var guessForT = intervalStart + dist * kSampleStepSize;

                var initialSlope = GetSlope(guessForT, mX1, mX2);
                if (initialSlope >= NEWTON_MIN_SLOPE)
                {
                    return NewtonRaphsonIterate(aX, guessForT, mX1, mX2);
                }
                else if (initialSlope == 0.0f)
                {
                    return guessForT;
                }
                else
                {
                    return BinarySubdivide(aX, intervalStart, intervalStart + kSampleStepSize, mX1, mX2);
                }
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
            } while (Math.Abs(currentX) > SUBDIVISION_PRECISION && ++i < SUBDIVISION_MAX_ITERATIONS);
            return currentT;
        }

        private static float NewtonRaphsonIterate(float aX, float aGuessT, float mX1, float mX2)
        {
            for (var i = 0; i < NEWTON_ITERATIONS; ++i)
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
}
