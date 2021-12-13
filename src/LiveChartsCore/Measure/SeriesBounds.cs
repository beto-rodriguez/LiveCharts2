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

using LiveChartsCore.Kernel;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the series bounds class, it contains the data bounds in a series.
/// </summary>
public class SeriesBounds
{
    private readonly bool _isPrevious;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeriesBounds"/> class.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <param name="isPrevious">if set to <c>true</c> [is previous].</param>
    public SeriesBounds(DimensionalBounds bounds, bool isPrevious)
    {
        Bounds = bounds;
        _isPrevious = HasData;
    }

    /// <summary>
    /// Gets the bounds.
    /// </summary>
    /// <value>
    /// The bounds.
    /// </value>
    public DimensionalBounds Bounds { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is previous.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is previous; otherwise, <c>false</c>.
    /// </value>
    public bool HasData => _isPrevious || Bounds.IsEmpty;
}
