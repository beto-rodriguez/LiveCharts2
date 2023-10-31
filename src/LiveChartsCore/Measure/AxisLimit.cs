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

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the axis limit structure.
/// </summary>
public struct AxisLimit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLimit"/> struct.
    /// </summary>
    /// <param name="min">The min value.</param>
    /// <param name="max">The masx value.</param>
    /// <param name="minDelta">The min delta.</param>
    /// <param name="dataMin">The data min value.</param>
    /// <param name="dataMax">The data max value.</param>
    public AxisLimit(double min, double max, double minDelta, double dataMin, double dataMax)
    {
        Min = min;
        Max = max;
        MinDelta = minDelta;
        DataMin = dataMin;
        DataMax = dataMax;
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double DataMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double DataMax { get; set; }

    /// <summary>
    /// Gets or sets the min delta.
    /// </summary>
    public double MinDelta { get; set; }

    internal static void ValidateLimits(ref double min, ref double max)
    {
        var isMax =
            min is double.MaxValue or double.MinValue ||
            max is double.MaxValue or double.MinValue;

        // easy workaround to prevent the axis from crashing
        // https://github.com/beto-rodriguez/LiveCharts2/issues/1294
        if (min > max)
        {
            var temp = max;

            max = min;
            min = temp;
        }

        if (!isMax) return;

        min = 0;
        max = 10;
    }
}
