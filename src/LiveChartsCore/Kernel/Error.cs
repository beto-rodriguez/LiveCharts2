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
/// Defines an error point.
/// </summary>
public readonly struct Error
{
    /// <summary>
    /// Initalizes a new instance of the <see cref="Error"/> struct.
    /// </summary>
    /// <param name="xi">The left error in the x axis.</param>
    /// <param name="xj">The right error in the x axis.</param>
    /// <param name="yi">The top error in the Y axis.</param>
    /// <param name="yj">The bottom error in the Y axis.</param>
    public Error(double xi, double xj, double yi, double yj)
    {
        Xi = xi;
        Xj = xj;
        Yi = yi;
        Yj = yj;
    }

    /// <summary>
    /// Initalizes a new instance of the <see cref="Error"/> struct.
    /// </summary>
    /// <param name="x">The error in x.</param>
    /// <param name="y">The error in y.</param>
    public Error(double x, double y) : this(x, x, y, y)
    { }

    internal Error(bool isEmpty)
    {
        IsEmpty = isEmpty;
    }

    /// <summary>
    /// Gets the error to the left of the X axis.
    /// </summary>
    public readonly double Xi { get; }

    /// <summary>
    /// Gets the error to the right of the X axis.
    /// </summary>
    public readonly double Xj { get; }

    /// <summary>
    /// Gets the error to the top of the Y axis.
    /// </summary>
    public readonly double Yi { get; }

    /// <summary>
    /// Gets the error to the bottom of Y axis.
    /// </summary>
    public readonly double Yj { get; }

    /// <summary>
    /// Gets whether the error is empty.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Gets an empty instance of the error class.
    /// </summary>
    public static Error Empty => new(true);
}
