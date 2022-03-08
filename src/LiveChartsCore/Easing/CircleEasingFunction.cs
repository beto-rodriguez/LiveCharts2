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
/// Defines the CircleEasingFunction
/// </summary>
public static class CircleEasingFunction
{
    /// <summary>
    /// the ease in.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float In(float t)
    {
        unchecked
        {
            return (float)(1 - Math.Sqrt(1 - t * t));
        }
    }

    /// <summary>
    /// the ease out.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float Out(float t)
    {
        unchecked
        {
            return (float)Math.Sqrt(1 - (--t * t));
        }
    }

    /// <summary>
    /// the ease in out
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float InOut(float t)
    {
        return (float)((t *= 2) <= 1 ? 1 - Math.Sqrt(1 - t * t) : Math.Sqrt(1 - (t -= 2) * t) + 1) / 2f;
    }
}
