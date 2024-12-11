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
/// Defines a stepline visual point.
/// </summary>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
public class SegmentVisualPoint<TVisual, TSegment>
    where TVisual : BoundedDrawnGeometry, new()
    where TSegment : Segment, new()
{
    /// <summary>
    /// Gets the geometry.
    /// </summary>
    /// <value>
    /// The geometry.
    /// </value>
    public TVisual Geometry { get; set; } = new();

    /// <summary>
    /// Gets the stepline.
    /// </summary>
    /// <value>
    /// The stepline.
    /// </value>
    public TSegment Segment { get; set; } = new();

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public BaseVectorGeometry<TSegment>? FillPath { get; set; }

    /// <summary>
    /// Gets or sets the stroke path.
    /// </summary>
    /// <value>
    /// The stroke path.
    /// </value>
    public BaseVectorGeometry<TSegment>? StrokePath { get; set; }
}

/// <summary>
/// Defines a stepline visual point.
/// </summary>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
/// <typeparam name="TErrorGeometry">The type of the error geometry.</typeparam>
public class SegmentVisualPoint<TVisual, TSegment, TErrorGeometry>
    : SegmentVisualPoint<TVisual, TSegment>
        where TVisual : BoundedDrawnGeometry, new()
        where TSegment : Segment, new()
        where TErrorGeometry : DrawnGeometry
{
    /// <summary>
    /// Gets or sets the y error geometry.
    /// </summary>
    public TErrorGeometry? YError { get; set; }

    /// <summary>
    /// Gets or sets the x error geometry.
    /// </summary>
    public TErrorGeometry? XError { get; set; }
}
