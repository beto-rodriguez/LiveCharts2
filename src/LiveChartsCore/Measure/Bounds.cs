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
/// Represents the maximum and minimum values in a set.
/// </summary>
public class Bounds
{
    /// <summary>
    /// Creates a new instance of the <see cref="Bounds"/> class.
    /// </summary>
    public Bounds()
    { }

    /// <summary>
    /// Creates a new instance of the <see cref="Bounds"/> class.
    /// </summary>
    /// <param name="max">The maximum value.</param>
    /// <param name="min">The minimum value.</param>
    public Bounds(double min, double max)
    {
        Max = max;
        Min = min;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Bounds"/> class. based on the given <see cref="Bounds"/>.
    /// </summary>
    /// <param name="bounds"></param>
    public Bounds(Bounds bounds)
    {
        IsEmpty = bounds.IsEmpty;
        Max = bounds.Max;
        Min = bounds.Min;
        PaddingMax = bounds.PaddingMax;
        PaddingMin = bounds.PaddingMin;
        RequestedGeometrySize = bounds.RequestedGeometrySize;
        MinDelta = bounds.MinDelta;
    }

    /// <summary>
    /// Gets whether the bounds are empty.
    /// </summary>
    public bool IsEmpty { get; internal set; } = true;

    /// <summary>
    /// Gets the maximum value in the data set.
    /// </summary>
    public double Max { get; internal set; } = double.MinValue;

    /// <summary>
    /// Gets the minimum value in the data set.
    /// </summary>
    public double Min { get; internal set; } = double.MaxValue;

    /// <summary>
    /// Gets the padding, the distance from the edge to the first point in the series.
    /// </summary>
    public double PaddingMax { get; internal set; } = 0;

    /// <summary>
    /// Gets the padding, the distance from the edge to the last point in the series.
    /// </summary>
    public double PaddingMin { get; internal set; } = 0;

    /// <summary>
    /// Gets the requested geometry size.
    /// </summary>
    public double RequestedGeometrySize { get; internal set; } = 0;

    /// <summary>
    /// Gets the delta, the absolute range in the axis.
    /// </summary>
    /// <value>
    /// The delta.
    /// </value>
    public double Delta => Max - Min;

    /// <summary>
    /// Gets the minimum delta.
    /// </summary>
    /// <value>
    /// The minimum delta.
    /// </value>
    public double MinDelta { get; internal set; } = double.MaxValue;

    /// <summary>
    /// Compares the current bounds with a given value,
    /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
    /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
    /// </summary>
    /// <param name="value">the value to append</param>
    internal void AppendValue(double value)
    {
        if (Max <= value) Max = value;
        if (Min >= value) Min = value;
        IsEmpty = false;
    }

    /// <summary>
    /// Compares the current bounds with a given value,
    /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
    /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
    /// </summary>
    /// <param name="bounds">the bounds to append</param>
    internal void AppendValue(Bounds bounds)
    {
        if (Max < bounds.Max) Max = bounds.Max;
        if (Min > bounds.Min) Min = bounds.Min;
        if (bounds.MinDelta < MinDelta) MinDelta = bounds.MinDelta;
        if (RequestedGeometrySize < bounds.RequestedGeometrySize) RequestedGeometrySize = bounds.RequestedGeometrySize;
        if (PaddingMin < bounds.PaddingMin) PaddingMin = bounds.PaddingMin;
        if (PaddingMax < bounds.PaddingMax) PaddingMax = bounds.PaddingMax;
        IsEmpty = false;
    }
}
