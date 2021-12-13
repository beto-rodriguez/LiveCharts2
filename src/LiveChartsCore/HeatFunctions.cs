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
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines common heat functions.
/// </summary>
public static class HeatFunctions
{
    /// <summary>
    /// Builds the color stops.
    /// </summary>
    /// <param name="heatMap">The heat map.</param>
    /// <param name="colorStops">The color stops.</param>
    /// <returns></returns>
    /// <exception cref="Exception">
    /// At least 2 colors are required in a heat map.
    /// or
    /// At least 2 colors are required in a heat map.
    /// </exception>
    public static List<Tuple<double, LvcColor>> BuildColorStops(LvcColor[] heatMap, double[]? colorStops)
    {
        if (heatMap.Length < 2) throw new Exception("At least 2 colors are required in a heat map.");

        if (colorStops is null)
        {
            var s = 1 / (double)(heatMap.Length - 1);
            colorStops = new double[heatMap.Length];
            var x = 0d;
            for (var i = 0; i < heatMap.Length; i++)
            {
                colorStops[i] = x;
                x += s;
            }
        }

        if (colorStops.Length != heatMap.Length)
            throw new Exception($"ColorStops and HeatMap must have the same length.");

        var heatStops = new List<Tuple<double, LvcColor>>();
        for (var i = 0; i < colorStops.Length; i++)
        {
            heatStops.Add(new Tuple<double, LvcColor>(colorStops[i], heatMap[i]));
        }

        return heatStops;
    }

    /// <summary>
    /// Interpolates the color.
    /// </summary>
    /// <param name="weight">The weight.</param>
    /// <param name="weightBounds">The weight bounds.</param>
    /// <param name="heatMap">The heat map.</param>
    /// <param name="heatStops">The heat stops.</param>
    /// <returns></returns>
    public static LvcColor InterpolateColor(float weight, Bounds weightBounds, LvcColor[] heatMap, List<Tuple<double, LvcColor>> heatStops)
    {
        var range = weightBounds.Max - weightBounds.Min;
        if (range == 0) range = double.Epsilon;
        var p = (weight - weightBounds.Min) / range;
        if (p < 0) p = 0;
        if (p > 1) p = 1;

        var previous = heatStops[0];

        for (var i = 1; i < heatStops.Count; i++)
        {
            var next = heatStops[i];

            if (next.Item1 < p)
            {
                previous = heatStops[i];
                continue;
            }

            var px = (p - previous.Item1) / (next.Item1 - previous.Item1);

            return LvcColor.FromArgb(
                (byte)(previous.Item2.A + px * (next.Item2.A - previous.Item2.A)),
                (byte)(previous.Item2.R + px * (next.Item2.R - previous.Item2.R)),
                (byte)(previous.Item2.G + px * (next.Item2.G - previous.Item2.G)),
                (byte)(previous.Item2.B + px * (next.Item2.B - previous.Item2.B)));
        }

        return heatMap[heatMap.Length - 1];
    }
}
