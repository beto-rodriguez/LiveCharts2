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
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Measure;

/// <summary>
/// A helper class to get the clipping rectangle.
/// </summary>
public static class Clipping
{
    /// <summary>
    /// Calculates the clipping rectangle based on a clipping mode.
    /// </summary>
    /// <param name="mode">The mode.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static LvcRectangle GetClipRectangle(
        ClipMode mode, Chart chart)
    {
        var x = chart.DrawMarginLocation.X;
        var y = chart.DrawMarginLocation.Y;
        var w = x + chart.DrawMarginSize.Width;
        var h = y + chart.DrawMarginSize.Height;

        return mode switch
        {
            ClipMode.None =>
                new(new(), chart.ControlSize),
            ClipMode.X =>
                new(
                    new(x, 0),
                    new(w, h)),
            ClipMode.Y =>
                new(
                    new(0, y),
                    new(w, h)),
            ClipMode.XY =>
                new(
                    new(x, y),
                    new(w, h)),

            _ => throw new NotImplementedException(),
        };
    }
}
