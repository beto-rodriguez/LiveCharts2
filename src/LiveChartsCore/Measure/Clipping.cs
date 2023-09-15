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
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="mode">The mode.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static LvcRectangle GetClipRectangle<TDrawingContext>(
        ClipMode mode, Chart<TDrawingContext> chart)
            where TDrawingContext : DrawingContext
    {
        return mode switch
        {
            ClipMode.None =>
                new LvcRectangle(new LvcPoint(), chart.ControlSize),
            ClipMode.X =>
                new LvcRectangle(
                    new LvcPoint(chart.DrawMarginLocation.X, 0),
                    new LvcSize(chart.DrawMarginSize.Width, chart.ControlSize.Height)),
            ClipMode.Y =>
                new LvcRectangle(
                    new LvcPoint(0, chart.DrawMarginLocation.Y),
                    new LvcSize(chart.ControlSize.Width, chart.DrawMarginSize.Height)),
            ClipMode.XY =>
                new LvcRectangle(chart.DrawMarginLocation, chart.DrawMarginSize),

            _ => throw new NotImplementedException(),
        };
    }
}
