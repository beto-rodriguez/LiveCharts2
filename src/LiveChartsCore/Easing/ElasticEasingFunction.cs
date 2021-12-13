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
