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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a point with double precision.
/// </summary>
public struct LvcPointD
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LvcPointD"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public LvcPointD(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LvcPointD"/> struct.
    /// </summary>
    /// <param name="point">The point coordinate.</param>
    public LvcPointD(LvcPoint point)
    {
        X = point.X;
        Y = point.Y;
    }

    /// <summary>
    /// Gets or sets the X coordinate.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Determines whether the instance is equals to the given instance.
    /// </summary>
    /// <param name="obj">The instance to compare to.</param>
    /// <returns>The comparision result.</returns>
    public override bool Equals(object? obj)
    {
        return obj is LvcPoint point &&
            X == point.X &&
            Y == point.Y;
    }

    /// <summary>
    /// Gets the object hash code.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// Compares two <see cref="LvcPointD"/> instances.
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static bool operator ==(LvcPointD l, LvcPointD r) => l.Equals(r);

    /// <summary>
    /// Compares two <see cref="LvcPointD"/> instances.
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static bool operator !=(LvcPointD l, LvcPointD r) => !(l == r);
}
