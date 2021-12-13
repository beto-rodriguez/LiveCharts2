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

namespace LiveChartsCore.Easing;

/// <summary>
/// Defines the BounceEasingFunction
/// </summary>
public static class BounceEasingFunction
{
    private static readonly float s_b1 = 4f / 11f;
    private static readonly float s_b2 = 6f / 11f;
    private static readonly float s_b3 = 8f / 11f;
    private static readonly float s_b4 = 3f / 4f;
    private static readonly float s_b5 = 9f / 11f;
    private static readonly float s_b6 = 10f / 11f;
    private static readonly float s_b7 = 15f / 16f;
    private static readonly float s_b8 = 21f / 22f;
    private static readonly float s_b9 = 63f / 64f;
    private static readonly float s_b0 = 1f / s_b1 / s_b1;

    /// <summary>
    /// the in easing.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float In(float t)
    {
        return 1 - Out(1 - t);
    }

    /// <summary>
    /// the out easing.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float Out(float t)
    {
        return (t = +t) < s_b1
            ? s_b0 * t * t : t < s_b3 ? s_b0 * (t -= s_b2) * t + s_b4 : t < s_b6 ? s_b0 * (t -= s_b5) * t + s_b7
            : s_b0 * (t -= s_b8) * t + s_b9;
    }

    /// <summary>
    /// the in out easing.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    public static float InOut(float t)
    {
        return ((t *= 2) <= 1 ? 1 - Out(1 - t) : Out(t - 1) + 1) / 2f;
    }
}
