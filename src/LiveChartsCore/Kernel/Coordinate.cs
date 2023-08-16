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

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines a coordinate.
/// </summary>
public readonly struct Coordinate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> struct.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="thirdQuartile">The third quiartile value.</param>
    /// <param name="firstQuartile">the first quartile value.</param>
    /// <param name="minimum">the minimum value.</param>
    /// <param name="median">the median value.</param>
    public Coordinate(
        double x, double maximum, double thirdQuartile,
        double firstQuartile, double minimum, double median) : this()
    {
        PrimaryValue = maximum;
        SecondaryValue = x;
        TertiaryValue = thirdQuartile;
        QuaternaryValue = firstQuartile;
        QuinaryValue = minimum;
        SenaryValue = median;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> struct.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="high">The high value.</param>
    /// <param name="open">The open value.</param>
    /// <param name="close">The quaternary value.</param>
    /// <param name="low">The quinary value.</param>
    public Coordinate(double x, double high, double open, double close, double low)
        : this(x, high, open, close, low, 0)
    {
        PrimaryValue = high;
        SecondaryValue = x;
        TertiaryValue = open;
        QuaternaryValue = close;
        QuinaryValue = low;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Coordinate(double x, double y) : this(x, y, 0, 0, 0)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="weight">The weight of the pint.</param>
    public Coordinate(double x, double y, double weight) : this(x, y, weight, 0, 0)
    { }

    private Coordinate(bool isEmpty) : this(0, 0, 0, 0, 0)
    {
        IsEmpty = isEmpty;
    }

    /// <summary>
    /// Gets an empty coordinate instance.
    /// </summary>
    public static Coordinate Empty => new(true);

    /// <summary>
    /// Evaluates whether the instance is empty.
    /// </summary>
    public readonly bool IsEmpty { get; }

    /// <summary>
    /// Gets or sets the primary value, normally the Y coordinate or the value in a gauge.
    /// </summary>
    /// <value>
    /// The primary value.
    /// </value>
    public readonly double PrimaryValue { get; }

    /// <summary>
    /// Gets or sets the secondary value, normally the X coordinate.
    /// </summary>
    /// <value>
    /// The secondary value.
    /// </value>
    public readonly double SecondaryValue { get; }

    /// <summary>
    /// Gets or sets the tertiary value, normally used on weighted, financial or box series.
    /// </summary>
    /// <value>
    /// The tertiary value.
    /// </value>
    public readonly double TertiaryValue { get; }

    /// <summary>
    /// Gets or sets the quaternary value, normally used on financial or box series.
    /// </summary>
    /// <value>
    /// The quaternary value.
    /// </value>
    public readonly double QuaternaryValue { get; }

    /// <summary>
    /// Gets or sets the quinary value, normally used on financial or box series.
    /// </summary>
    /// <value>
    /// The quinary value.
    /// </value>
    public readonly double QuinaryValue { get; }

    /// <summary>
    /// Gets or sets the senary value, normally used on financial or box series.
    /// </summary>
    /// <value>
    /// The senary value.
    /// </value>
    public readonly double SenaryValue { get; }
}
