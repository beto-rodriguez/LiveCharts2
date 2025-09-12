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

namespace LiveChartsCore.Drawing.Segments;

/// <summary>
/// Defines a point in a segment that is part of a path.
/// </summary>
/// <param name="geometry">The drawn geometry for the point.</param>
public class SegmentVisualPoint(BoundedDrawnGeometry geometry)
{
    /// <summary>
    /// Gets the drawn geometry for the point.
    /// </summary>
    public BoundedDrawnGeometry Geometry { get; } = geometry;

    /// <summary>
    /// Gets the segment in the path that this point belongs to.
    /// </summary>
    public Segment Segment { get; } = new Segment();

    /// <summary>
    /// Gets the X error geometry.
    /// </summary>
    public BaseLineGeometry? XError { get; internal set; }

    /// <summary>
    /// Gets the Y error geometry.
    /// </summary>
    public BaseLineGeometry? YError { get; internal set; }
}
